using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LocalCustomizationStorage : ICustomizationStorage
{
    private readonly string filePath = Path.Combine(Application.persistentDataPath, "customization.json");

    public void Save(CustomizationData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log($"[Save] 저장 완료: {filePath}");
    }

    public CustomizationData Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("[Load] 저장 파일 없음");
            return new CustomizationData();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<CustomizationData>(json);
    }
    
    public bool HasData()
    {
        return File.Exists(filePath);
    }
}