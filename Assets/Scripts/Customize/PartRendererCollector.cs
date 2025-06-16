using System.Collections.Generic;
using UnityEngine;

public class PartRendererCollector : MonoBehaviour
{
    private List<PartRendererEntry> entries;

    void Awake()
    {
        entries = new List<PartRendererEntry>();
        
        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var partType = GuessPartTypeFromName(renderer.name);
            entries.Add(new PartRendererEntry { partType = partType, renderer = renderer });
        }
    }
    
    private void Start()
    {
        CustomizationManager.Instance.RegisterRenderers(entries);
        CustomizationManager.Instance.LoadCustomization();
    }
    
    private PartType GuessPartTypeFromName(string name)
    {
        name = name.ToLower();

        if (name.Contains("full")) return PartType.FullBody;
        if (name.Contains("body")) return PartType.Body;
        if (name.Contains("hair")) return PartType.Hair;
        if (name.Contains("hat")) return PartType.Hat;
        if (name.Contains("glass")) return PartType.Glasses;
        if (name.Contains("backpack")) return PartType.Backpack;
        if (name.Contains("eyebrow")) return PartType.Eyebrow;
        if (name.Contains("glove")) return PartType.Glove;
        if (name.Contains("mustache")) return PartType.Mustache;
        if (name.Contains("outerwear")) return PartType.Outerwear;
        if (name.Contains("pants")) return PartType.Pants;
        if (name.Contains("shoe")) return PartType.Shoe;

        Debug.LogWarning($"[PartRendererCollector] 이름에서 PartType 추정 실패: {name}");
        return PartType.Body; // 기본값은 필요 시 조정
    }
}