#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || DEVELOPMENT_BUILD)
using System;
using System.Collections;
using System.Linq;
using Artel;
using GyeMong.GameSystem.Creature.Mob;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.GameSystem.Map.Stage.ScriptableObject;
using GyeMong.GameSystem.Map.Stage.Select;
using GyeMong.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using Object = UnityEngine.Object;

namespace GyeMong.QA
{
    public enum CombatStage { Slime, Elf, Shadow, Golem, NagaRogue, NagaWarrior, Wanderer }
    public enum EnemySelection { Nearest, All }

    public static class GyeMongGameFunctions
    {
        private const string CombatGroup = "player_combat";
        private static bool changingStage;
        private static bool defeatingEnemies;

        private static string HasPlayer()
        {
            var player = SceneContext.Character;
            return player != null && player.isActiveAndEnabled && player.CurrentHp > 0
                ? null : "No living player in the current scene.";
        }

        private static string CanChangeState()
        {
            if (changingStage || defeatingEnemies) return "Another QA state change is running.";
            if (Time.timeScale <= 0) return "Resume the game first.";
            var player = SceneContext.Character;
            return player != null && player.CurrentHp > 0 && player.isControlled ? "Wait for the cutscene." : null;
        }

        private static string HasEnemies()
        {
            return CanChangeState() ?? HasPlayer() ?? (LiveEnemies().Length == 0 ? "No living enemies." : null);
        }

        private static Mob[] LiveEnemies()
        {
            return Object.FindObjectsOfType<Mob>().Where(m => m.isActiveAndEnabled && m.CurrentHp > 0).ToArray();
        }

        private static Mob[] SelectEnemies(EnemySelection selection)
        {
            var player = SceneContext.Character;
            var enemies = LiveEnemies().OrderBy(m => ((Vector2)(m.transform.position - player.transform.position)).sqrMagnitude)
                .ThenBy(m => m.GetInstanceID());
            return selection == EnemySelection.All ? enemies.ToArray() : enemies.Take(1).ToArray();
        }

        [ArtelCheat("Heals the living player through the game's Heal method; no resurrection.", when: nameof(HasPlayer))]
        public static string HealPlayer([CheatRange(1, 10000)] float amount = 10000)
        {
            var player = SceneContext.Character;
            player.Heal(amount);
            return $"hp={player.CurrentHp}/{player.stat.HealthMax}";
        }

        [ArtelCheat("Sets living player HP/gauge percentages and shield for test setup, updating HUD. Does not change max stats.", when: nameof(HasPlayer))]
        public static string SetPlayerResources([CheatRange(1, 100)] float healthPercent = 100,
            [CheatRange(0, 100)] float gaugePercent = 100, [CheatRange(0, 1000)] float shield = 0)
        {
            var player = SceneContext.Character;
            player.SetQaResources(healthPercent, gaugePercent, shield);
            return $"hp={player.CurrentHp}; gauge={player.CurrentSkillGauge}; shield={player.curShield}";
        }

        [ArtelCheat("Reduces enemy HP by amount (capped at current phase HP), bypassing shields/front defense. Runs normal phase/death logic; next phase may refill HP. Nearest is measured from the player.", when: nameof(HasEnemies))]
        public static string DamageEnemies([CheatRange(0.1, 1000000)] float amount, EnemySelection selection = EnemySelection.Nearest)
        {
            var enemies = SelectEnemies(selection);
            var results = new System.Collections.Generic.List<string>();
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.isActiveAndEnabled || enemy.CurrentHp <= 0) continue;
                string label = enemy.name;
                float before = enemy.CurrentHp;
                enemy.ApplyQaHpDamage(amount);
                results.Add($"{label}: hp_before={before}, hp_after={(enemy == null ? 0 : enemy.CurrentHp)}");
            }
            return string.Join("; ", results);
        }

        [ArtelCheat("Defeats the selected enemy through normal damage/death/phase transitions. All also clears spawned split enemies. Waits for cutscenes, stops on scene change, fails after 45 seconds. Not for victory tests.", when: nameof(HasEnemies))]
        public static IEnumerator DefeatEnemies(EnemySelection selection = EnemySelection.Nearest)
        {
            var selected = SelectEnemies(selection);
            int scene = SceneManager.GetActiveScene().handle;
            float deadline = Time.realtimeSinceStartup + 45;
            float emptySince = -1;
            defeatingEnemies = true;
            try
            {
                while (SceneManager.GetActiveScene().handle == scene)
                {
                    if (Time.realtimeSinceStartup > deadline) throw new InvalidOperationException("Enemy defeat timed out; inspect the current phase/cutscene.");
                    var player = SceneContext.Character;
                    if (player == null || player.CurrentHp <= 0) yield break;
                    if (player.isControlled || Time.timeScale <= 0) { yield return null; continue; }
                    var targets = selection == EnemySelection.All ? LiveEnemies() : selected.Where(m => m != null && m.isActiveAndEnabled && m.CurrentHp > 0).ToArray();
                    if (targets.Length == 0)
                    {
                        if (emptySince < 0) emptySince = Time.realtimeSinceStartup;
                        if (Time.realtimeSinceStartup - emptySince >= 1.5f) yield break;
                    }
                    else
                    {
                        emptySince = -1;
                        foreach (var enemy in targets)
                        {
                            if (enemy != null && enemy.isActiveAndEnabled && enemy.CurrentHp > 0)
                                enemy.ApplyQaHpDamage(enemy.CurrentHp);
                            if (player.isControlled || !player.isActiveAndEnabled) break;
                        }
                    }
                    // Let phase event coroutines acquire their control lock before another hit.
                    float resumeAt = Time.realtimeSinceStartup + 0.5f;
                    while (Time.realtimeSinceStartup < resumeAt) yield return null;
                }
            }
            finally { defeatingEnemies = false; }
        }

        [ArtelCheat("Unlocks all seven combat stages in the save and opens stage selection using the game's loader.", when: nameof(CanChangeState))]
        public static IEnumerator UnlockStages()
        {
            changingStage = true;
            float deadline = Time.realtimeSinceStartup + 20;
            try
            {
                StageSelectPage.LoadStageSelectPageOnStageToDestination(Stage.Slime, Stage.Wanderer);
                PlayerPrefs.Save();
                while (SceneManager.GetActiveScene().name != "StageSelectPage")
                {
                    CheckStageDeadline(deadline);
                    yield return null;
                }
            }
            finally { changingStage = false; }
        }

        [ArtelCheat("Changes combat stage through StageManager and its normal intro. Updates saved stage selection/unlock progress and tutorial flag. Waits for a living player; not for progression/save tests.", when: nameof(CanChangeState))]
        public static IEnumerator GoToStage(CombatStage stage)
        {
            string[] paths = { "Spring/SlimeStage", "Spring/ElfStage", "Spring/ShadowStage", "Spring/GolemStage",
                "Summer/NagaRouge", "Summer/NagaWarrior", "Summer/Wanderer" };
            string[] scenes = { "SpringBeach", "SpringNorthForest", "SpringShadowIsland", "SpringGolemTemple",
                "NagaRogueScene", "SummerNagaWarrior", "Wanderer" };
            var info = Resources.Load<StageInfo>("ScriptableObjects/Stage/" + paths[(int)stage]);
            if (info == null) throw new InvalidOperationException("StageInfo is missing for " + stage);
            changingStage = true;
            float deadline = Time.realtimeSinceStartup + 50;
            try
            {
                SceneContext.Character?.ReleaseQaInput();
                PlayerPrefs.SetInt("TutorialFlag", 1);
                // Keep both indices within the selection scene's eight nodes and suppress its first-run popup.
                StageSelectPage.LoadStageSelectPageOnStageToDestination(info.id, (Stage)Mathf.Clamp(
                    Mathf.Max(PlayerPrefs.GetInt(StageSelectPage.MAX_STAGE_ID_KEY, 1), (int)info.id), 1, 7));
                PlayerPrefs.Save();
                while (SceneManager.GetActiveScene().name != "StageSelectPage")
                {
                    CheckStageDeadline(deadline);
                    yield return null;
                }
                yield return null;
                yield return null;
                StageManager.EnterStage(info, SceneLoader.Instance);
                while (SceneManager.GetActiveScene().name != scenes[(int)stage] || HasPlayer() != null)
                {
                    CheckStageDeadline(deadline);
                    yield return null;
                }
            }
            finally { changingStage = false; }
        }

        private static void CheckStageDeadline(float deadline)
        {
            if (Time.realtimeSinceStartup > deadline) throw new InvalidOperationException("Stage entry timed out. Finish the dialogue and inspect the stage configuration.");
        }

        private enum CombatMode { Attack, Chase, Flee, Kite, Circle, Skill, Recover }

        [ArtelBehavior("Continuously attacks toward the nearest enemy through player combat code; does not chase. Respects cooldowns/locks. Not an input binding test.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator KeepAttacking(ArtelBehaviorContext context) => RunCombat(CombatMode.Attack, 2, false, context);

        [ArtelBehavior("Chases the nearest enemy and attacks within attackDistance through player combat code. Local wall avoidance, not pathfinding.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator ChaseAndAttack([CheatRange(0.5, 5)] float attackDistance, ArtelBehaviorContext context) => RunCombat(CombatMode.Chase, attackDistance, false, context);

        [ArtelBehavior("Continuously retreats from nearby enemies until safeDistance, watching for new threats. Optional normal cooldown-limited dash; no invulnerability cheat.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator KeepFleeing([CheatRange(2, 20)] float safeDistance, bool useDash, ArtelBehaviorContext context) => RunCombat(CombatMode.Flee, safeDistance, useDash, context);

        [ArtelBehavior("Maintains distance from nearest enemy and fires normal skill attacks when gauge permits; retreats when too close. No resource refill.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator KeepDistance([CheatRange(2, 15)] float distance, ArtelBehaviorContext context) => RunCombat(CombatMode.Kite, distance, false, context);

        [ArtelBehavior("Circles nearest enemy clockwise at radius while aiming at it, using normal movement and local wall avoidance.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator CircleEnemy([CheatRange(1, 15)] float radius, ArtelBehaviorContext context) => RunCombat(CombatMode.Circle, radius, false, context);

        [ArtelBehavior("Continuously aims and uses uncharged skill attacks through the player's normal resource checks, without movement or gauge refill.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator KeepUsingSkill(ArtelBehaviorContext context) => RunCombat(CombatMode.Skill, 2, false, context);

        [ArtelBehavior("Retreats to a safe distance then holds the normal heal action until full HP. Uses gauge normally and fails if gauge is insufficient.", group: CombatGroup, when: nameof(HasPlayer))]
        public static IEnumerator RetreatAndHeal([CheatRange(2, 15)] float safeDistance, ArtelBehaviorContext context) => RunCombat(CombatMode.Recover, safeDistance, false, context);

        private static IEnumerator RunCombat(CombatMode mode, float distance, bool useDash, ArtelBehaviorContext context)
        {
            var player = SceneContext.Character;
            int scene = SceneManager.GetActiveScene().handle;
            float nextSkill = 0, nextDash = 0, nextScan = 0, nextReport = 0;
            bool previousHeal = false;
            Mob[] enemies = Array.Empty<Mob>();
            try
            {
                while (player != null && player.isActiveAndEnabled && player.CurrentHp > 0 && SceneManager.GetActiveScene().handle == scene)
                {
                    if (player.isControlled || Time.timeScale <= 0 || changingStage || defeatingEnemies)
                    {
                        player.ReleaseQaInput();
                        context.Report("waiting: player locked or QA state change");
                        yield return null;
                        continue;
                    }
                    if (Time.time >= nextScan)
                    {
                        enemies = LiveEnemies();
                        nextScan = Time.time + 0.2f;
                    }
                    var target = enemies.Where(m => m != null && m.isActiveAndEnabled && m.CurrentHp > 0)
                        .OrderBy(m => (m.transform.position - player.transform.position).sqrMagnitude).FirstOrDefault();
                    if (target == null && mode != CombatMode.Recover) { context.Complete("no living enemies"); yield break; }
                    Vector2 position = player.transform.position;
                    Vector2 aim = target != null ? (Vector2)target.transform.position : position + Vector2.up;
                    Vector2 delta = aim - position;
                    float range = target != null ? delta.magnitude : float.PositiveInfinity;
                    Vector2 toward = delta.sqrMagnitude > 0.001f ? delta.normalized : Vector2.up;
                    Vector2 move = Vector2.zero;
                    bool attack = false, skill = false, dash = false, heal = false;
                    if (mode == CombatMode.Attack) attack = true;
                    if (mode == CombatMode.Chase)
                    {
                        move = range > distance ? toward : Vector2.zero;
                        attack = range <= distance;
                    }
                    if (mode == CombatMode.Flee || mode == CombatMode.Recover)
                    {
                        if (range < distance) move = AwayFromEnemies(position, enemies, distance, -toward);
                    }
                    if (mode == CombatMode.Kite || mode == CombatMode.Circle)
                    {
                        move = range < distance - 0.5f ? -toward : range > distance + 0.5f ? toward : Vector2.zero;
                        if (mode == CombatMode.Circle) move = (move + new Vector2(toward.y, -toward.x)).normalized;
                    }
                    if ((mode == CombatMode.Skill || mode == CombatMode.Kite) && !player.isAttacking && !player.isDashing && Time.time >= nextSkill && player.CurrentSkillGauge >= player.stat.SkillCost)
                    {
                        // One-frame press releases the charge naturally; never overlap charging coroutines.
                        skill = true;
                        nextSkill = Time.time + Mathf.Max(1.5f, player.stat.AttackDelay + 0.2f);
                    }
                    move = AvoidWalls(position, move);
                    if (useDash && move.sqrMagnitude > 0 && !player.isDashing && Time.time >= nextDash)
                    {
                        dash = true;
                        nextDash = Time.time + Mathf.Max(0.5f, player.stat.DashCooldown + player.stat.DashDuration);
                    }
                    if (mode == CombatMode.Recover)
                    {
                        if (player.CurrentHp >= player.stat.HealthMax) { context.Complete("fully healed"); yield break; }
                        if (player.CurrentSkillGauge < player.stat.HealCost && !player.isAttacking)
                        { context.Fail("insufficient gauge to heal"); yield break; }
                        heal = range >= distance && (player.QaIsHealing || !previousHeal);
                        if (heal) move = Vector2.zero;
                    }
                    player.ApplyQaInput(move, aim, attack, skill, dash, heal);
                    previousHeal = heal;
                    if (Time.time >= nextReport)
                    {
                        context.Report($"{mode}; target={(target != null ? target.name : "none")}; distance={range:F1}; hp={player.CurrentHp:F1}; gauge={player.CurrentSkillGauge:F1}");
                        nextReport = Time.time + 0.5f;
                    }
                    yield return null;
                }
                context.Complete("player unavailable or scene changed");
            }
            finally { if (player != null) player.ReleaseQaInput(); }
        }

        private static Vector2 AwayFromEnemies(Vector2 position, Mob[] enemies, float radius, Vector2 fallback)
        {
            Vector2 away = Vector2.zero;
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.isActiveAndEnabled || enemy.CurrentHp <= 0) continue;
                Vector2 offset = position - (Vector2)enemy.transform.position;
                if (offset.sqrMagnitude < radius * radius) away += offset / Mathf.Max(0.1f, offset.sqrMagnitude);
            }
            return away.sqrMagnitude > 0.001f ? away.normalized : fallback;
        }

        private static Vector2 AvoidWalls(Vector2 position, Vector2 desired)
        {
            if (desired.sqrMagnitude < 0.001f) return Vector2.zero;
            Vector2 best = Vector2.zero;
            float bestScore = float.NegativeInfinity;
            int walls = LayerMask.GetMask("Wall");
            // Score the same eight directions the actual keyboard movement can produce.
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4;
                Vector2 candidate = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                if (Physics2D.CircleCast(position, 0.3f, candidate, 0.8f, walls).collider != null) continue;
                float score = Vector2.Dot(desired.normalized, candidate);
                if (score > bestScore) { bestScore = score; best = candidate; }
            }
            return bestScore >= 0 ? best : Vector2.zero;
        }
    }
}
#endif
