using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PartOptionsGenerator
{
    [MenuItem("Assets/Generate All Part Options From CustomizationData", priority = 0)]
    public static void GenerateAll()
    {
        // 선택한 CustomizationData 폴더 경로
        string rootFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!AssetDatabase.IsValidFolder(rootFolderPath))
        {
            Debug.LogError("폴더를 선택해주세요.");
            return;
        }

        // 하위 폴더 순회
        string[] subfolders = AssetDatabase.GetSubFolders(rootFolderPath);
        int createdCount = 0;

        foreach (string folder in subfolders)
        {
            string partTypeName = Path.GetFileName(folder);
            if (!System.Enum.TryParse(partTypeName, out PartType parsedType))
            {
                Debug.LogWarning($"[건너뜀] 폴더명 '{partTypeName}'은 PartType과 일치하지 않습니다.");
                continue;
            }

            List<MeshPartOption> options = new();

            // Empty 추가
            if (parsedType != PartType.Body)
            {
                options.Add(new MeshPartOption
                {
                    id = "Empty",
                    mesh = null
                });
            }

            // 해당 폴더 내의 Mesh 수집
            string[] guids = AssetDatabase.FindAssets("t:Mesh", new[] { folder });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                if (mesh == null) continue;

                var option = new MeshPartOption
                {
                    id = Path.GetFileNameWithoutExtension(assetPath),
                    mesh = mesh
                };

                options.Add(option);
            }

            // ScriptableObject 생성 및 저장
            PartOptionsGroup asset = ScriptableObject.CreateInstance<PartOptionsGroup>();
            asset.partType = parsedType;
            asset.options = options;

            string assetFileName = $"{partTypeName}_Options.asset";
            string assetSavePath = $"{rootFolderPath}/{assetFileName}";

            AssetDatabase.CreateAsset(asset, assetSavePath);
            EditorUtility.SetDirty(asset);
            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[CustomizationData] 총 {createdCount}개의 PartOptionsGroup.asset이 생성되었습니다.");
    }
}
