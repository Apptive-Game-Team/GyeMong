using System.Collections;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Component.Collider;
using GyeMong.GameSystem.Creature.Player.Controller;
using GyeMong.InputSystem;
using UnityEngine;
using DG.Tweening;

namespace GyeMong.GameSystem.Creature.Player
{
    public partial class PlayerCharacter : MonoBehaviour, IControllable, IEventTriggerable
    {
        public PlayerChangeListenerCaller changeListenerCaller = new PlayerChangeListenerCaller();
        public StatComponent stat;
        [SerializeField] private StatData _statData;
        [SerializeField] private float curHealth;
        [SerializeField] public float curShield;
        [SerializeField] private float curSkillGauge;
        public bool isControlled = false;
        
        public Vector2 Movement => movement;
        public Vector2 Velocity => playerRb.velocity;
        
        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Vector2 mousePosition;
        public Vector2 mouseDirection { get; private set; }
        
        public Rigidbody2D playerRb;
        private Animator animator;
        private PlayerSoundController soundController;
        
        public GameObject attackColliderPrefab;
        public GameObject attackComboColliderPrefab;
        public GameObject skillColliderPrefab;
        public GameObject healEffectPrefab;
        public GameObject healCompleteEffectPrefab;
        private GameObject activeHealEffect;
        private GameObject activeHealCompleteEffect;
        private float blinkDelay = 0.2f;

        private bool isMoving = false;     
        private bool canDash = true;
        public bool isDashing = false;
        public bool isAttacking = false;
        private bool isHealing = false;
        private bool canMove = true;
        public bool isInvincible = false;
        private bool canCombo = false;
        private bool comboQueued = false;
        private CircleCollider2D _hitCollider;

        public bool isTutorial;

        private Coroutine healingCoroutine;

        public Material[] materials;
        private Renderer _renderer;
        private SpriteRenderer _spriteRenderer;

        private Coroutine _attackCoroutine;
        private Tween _attackMoveTween;
        private Tween _dashTween;

        protected void Awake()
        {
            stat = _statData.GetStatComp();
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();
            _hitCollider = transform.Find("HitCollider").GetComponent<CircleCollider2D>();
            isTutorial = PlayerPrefs.GetInt("TutorialFlag") == 0;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _renderer = gameObject.GetComponent<Renderer>();
            _renderer.material = materials[0];
        }

        private void Start()
        {
            changeListenerCaller.CallShieldChangeListeners(curShield);
            changeListenerCaller.CallHpChangeListeners(curHealth);
        }

        private void Update()
        {
            if (!isControlled)
            {
                if (canMove)
                {
                    HandleMoveInput();
                    UpdateState();
                }
                HandleActionInput();
            }
        }

        private void FixedUpdate()
        {
            if (!isControlled)
            {
                MoveCharacter();
            }
        }

    }
}
