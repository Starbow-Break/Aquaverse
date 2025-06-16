using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartOptionsGroup", menuName = "Customization/Part Options Group")]
public class PartOptionsGroup : ScriptableObject
{
    public PartType partType;
    public List<MeshPartOption> options;
}

[System.Serializable]
public class MeshPartOption
{
    public string id;
    public Mesh mesh;
    
    public bool IsEmpty => mesh == null || id.ToLower().Contains("empty");
}