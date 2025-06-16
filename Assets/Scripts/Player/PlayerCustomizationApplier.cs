using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizationApplier : MonoBehaviour
{
    private List<PartRendererEntry> entries;
    private Dictionary<PartType, SkinnedMeshRenderer> renderers;

    void Awake()
    {
        entries = new List<PartRendererEntry>();
        renderers = new Dictionary<PartType, SkinnedMeshRenderer>();
        
        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var partType = GuessPartTypeFromName(renderer.name);
            entries.Add(new PartRendererEntry { partType = partType, renderer = renderer });
            renderers.TryAdd(partType, renderer);
        }
    }
    
    public void ApplyCustomization(CustomizationData data)
    {
        foreach (var kvp in data.partSelections)
        {
            if (!Enum.TryParse(kvp.Key, out PartType partType)) continue;
            var option = CustomizationManager.Instance.GetOption(partType, kvp.Value);
            if (option == null) continue;

            if (renderers.TryGetValue(partType, out var renderer))
            {
                renderer.sharedMesh = option.mesh;
                renderer.gameObject.SetActive(!option.IsEmpty);
            }
        }
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
