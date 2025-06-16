using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class PartRendererEntry
{
    public PartType partType;
    public SkinnedMeshRenderer renderer;
}

public class CustomizationManager : MonoBehaviour
{
    public static CustomizationManager Instance { get; private set; }
    
    private Dictionary<PartType, List<MeshPartOption>> _partOptions = new();
    public IReadOnlyDictionary<PartType, List<MeshPartOption>> PartOptions => _partOptions;
    
    private Dictionary<PartType, Dictionary<string, MeshPartOption>> _optionIndex;
    private Dictionary<PartType, SkinnedMeshRenderer> PartRenderers;
    
    public Dictionary<PartType, MeshPartOption> CurrentSelections;
    
    [SerializeField] private List<PartRendererEntry> partRendererEntries; //처음에 걍 적용시키려고 넣는 값
    
    private ICustomizationStorage storage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        storage = new LocalCustomizationStorage();

        RegisterRenderers(partRendererEntries);
        LoadAllOptions();
    }

    private void Start()
    {
        if (HasSavedData()) return;
        ApplyDefaultAll();
    }
    
    public void RegisterRenderers(List<PartRendererEntry> newEntries)
    {
        PartRenderers = new Dictionary<PartType, SkinnedMeshRenderer>();
        foreach (var entry in newEntries)
        {
            if (!PartRenderers.ContainsKey(entry.partType))
            {
                PartRenderers.Add(entry.partType, entry.renderer);
            }
        }
    }
    
    public MeshPartOption GetOption(PartType partType, string optionId)
    {
        if (_optionIndex.TryGetValue(partType, out var optionsById))
        {
            if (optionsById.TryGetValue(optionId, out var option))
            {
                return option;
            }
        }

        Debug.LogWarning($"[Customization] 옵션을 찾을 수 없음: {partType} / {optionId}");
        return null;
    }

    private void LoadAllOptions()
    {
        _partOptions = new Dictionary<PartType, List<MeshPartOption>>();
        _optionIndex = new Dictionary<PartType, Dictionary<string, MeshPartOption>>();
        CurrentSelections = new Dictionary<PartType, MeshPartOption>();

        var allGroups = Resources.LoadAll<PartOptionsGroup>("CustomizationData");
        foreach (var group in allGroups)
        {
            _partOptions[group.partType] = new List<MeshPartOption>(group.options);
            
            var index = new Dictionary<string, MeshPartOption>();
            foreach (var option in group.options)
            {
                index[option.id] = option;
            }
            _optionIndex[group.partType] = index;
        }

        Debug.Log($"[Customization] Loaded {PartOptions.Count} parts.");
    }
    
    private void ApplyDefaultAll()
    {
        foreach (var kvp in PartOptions)
        {
            var partType = kvp.Key;
            var options = kvp.Value;

            if (options.Count == 0) continue;

            var defaultOption = options[0]; // 항상 첫 번째 옵션을 기본값으로 사용
            ApplyOption(partType, defaultOption.id);
        }

        SaveCustomization();
        Debug.Log("[Customization] 기본 캐릭터 적용 완료");
    }

    public void ApplyOption(PartType partType, string optionId)
    {
        if (!_optionIndex.TryGetValue(partType, out var optionsById)) return;
        if (!optionsById.TryGetValue(optionId, out var option)) return;

        if (!PartRenderers.TryGetValue(partType, out var skinnedMeshRenderer)) return;

        if (option.IsEmpty)
        {
            skinnedMeshRenderer.gameObject.SetActive(false);
            CurrentSelections.Remove(partType);
        }
        else
        {
            skinnedMeshRenderer.sharedMesh = option.mesh;
            skinnedMeshRenderer.gameObject.SetActive(true);
            CurrentSelections[partType] = option;
        }
    }
    
    public void SaveCustomization()
    {
        var data = new CustomizationData
        {
            partSelections = new Dictionary<string, string>()
        };

        foreach (var partType in PartOptions.Keys)
        {
            if (CurrentSelections.TryGetValue(partType, out var option))
            {
                data.partSelections[partType.ToString()] = option.id;
            }
            else
            {
                data.partSelections[partType.ToString()] = "Empty"; 
            }
        }

        storage.Save(data);
    }
    
    public void LoadCustomization()
    {
        var data = storage.Load();
        foreach (var kvp in data.partSelections)
        {
            if (Enum.TryParse(kvp.Key, out PartType partType))
            {
                Debug.Log($"[Load] {partType}, {kvp.Value}");
                ApplyOption(partType, kvp.Value);
            }
            else
            {
                Debug.LogWarning($"[Customization] 알 수 없는 파츠 타입: {kvp.Key}");
            }
        }

        Debug.Log("[Customization] 저장된 데이터 로드 완료");
    }
    public string LoadRawAsJson()
    {
        var partDict = new Dictionary<string, string>();

        foreach (var partType in PartOptions.Keys)
        {
            if (CurrentSelections.TryGetValue(partType, out var option))
            {
                partDict[partType.ToString()] = option.id;
            }
            else
            {
                partDict[partType.ToString()] = "Empty";
            }
        }

        var data = new CustomizationData
        {
            partSelections = partDict
        };

        return JsonConvert.SerializeObject(data);
    }

    public CustomizationData ParseJsonToCustomizationData(string json)
    {
        return JsonConvert.DeserializeObject<CustomizationData>(json);
    }

    public bool HasSavedData()
    {
        return storage.HasData();
    }
    
    public bool IsCurrentSelectionEqualToSavedData()
    {
        if (!HasSavedData()) return false;
        
        var saved = storage.Load();

        foreach (var kvp in CurrentSelections)
        {
            var partType = kvp.Key;
            var optionId = kvp.Value.id;

            
            // 저장된 데이터에 이 파트가 없으면 다름
            if (!saved.partSelections.TryGetValue(partType.ToString(), out var savedId))
                return false;

            // ID가 다르면 다름
            if (savedId != optionId)
                return false;
            Debug.Log($"{savedId}, {optionId}");
        }

        return true;
    }
    
    public async UniTask SyncCurrentSelectionsFromSavedData()
    {
        SaveCustomization();
        
        await UniTask.Yield();
    }
    
    public void ClearPart(PartType partType)
    {
        if (PartRenderers.TryGetValue(partType, out var renderer))
        {
            renderer.gameObject.SetActive(false);
        }
        CurrentSelections.Remove(partType);
    }
}