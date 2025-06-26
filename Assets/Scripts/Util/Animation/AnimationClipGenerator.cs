#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AnimationClipGenerator
{
    private static int ExtractNumber(string name)
    {
        string num = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
        return int.TryParse(num, out int result) ? result : 0;
    }
    
    [MenuItem("Tools/Generate Clips from SpriteSheet Grid")]
    public static void Generate()
    {
        
        // 📁 설정
        string spriteSheetPath = "Assets/GyeMong_Art/Art/CHsprite/NagaRogue/badlizard_throw_sheet.png"; // 시트 경로
        int framePerDirection = 5; // 프레임 수 (가로 개수)
        int directionCount = 4; // 방향 수 (세로 개수)
        float frameRate = 12f;
        string[] directions = { "Down", "Left", "Up", "Right" };


        // 📦 스프라이트들 로드
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath);
        Sprite[] sprites = System.Array.FindAll(assets, x => x is Sprite).Cast<Sprite>().ToArray();
        
        if (sprites.Length != framePerDirection * directionCount)
        {
            Debug.LogError($"❌ 스프라이트 수가 안 맞음! 총 {sprites.Length}개, 기대값: {framePerDirection * directionCount}");
            return;
        }

        // 이름순 정렬
        System.Array.Sort(sprites, (a, b) =>
        {
            int aNum = ExtractNumber(a.name);
            int bNum = ExtractNumber(b.name);
            return aNum.CompareTo(bNum);
        });

        string baseFolder = Path.GetDirectoryName(spriteSheetPath);
        string saveFolder = baseFolder + "/Generated";
        Directory.CreateDirectory(saveFolder);

        for (int dirIndex = 0; dirIndex < directionCount; dirIndex++)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = frameRate;

            EditorCurveBinding binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[framePerDirection];
            for (int i = 0; i < framePerDirection; i++)
            {
                keys[i] = new ObjectReferenceKeyframe
                {
                    time = i / frameRate,
                    value = sprites[i * directionCount + dirIndex]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

            string clipName = $"Throw_{directions[dirIndex]}.anim";
            string savePath = $"{saveFolder}/{clipName}";
            AssetDatabase.CreateAsset(clip, savePath);
            Debug.Log($"✅ 생성됨: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
