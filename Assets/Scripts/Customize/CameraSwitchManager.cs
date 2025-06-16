using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    [SerializeField] private List<CinemachineCamera> cameraList;
    private Dictionary<string, CinemachineCamera> cameraDict;

    private void Awake()
    {
        cameraDict = new Dictionary<string, CinemachineCamera>();

        foreach (var cam in cameraList)
        {
            Debug.Log($"[CameraSwitch] Adding {cam.name}");
            cameraDict.Add(cam.name, cam);
        }
        
        SetCamera(cameraList[0]);
    }
    
    public void SetCamera(CinemachineCamera target)
    {
        foreach (var cam in cameraDict.Values)
        {
            cam.Priority = (cam == target) ? 10 : 0;
        }
    }
    
    public void SetCamera(string cameraName)
    {
        if (cameraDict.TryGetValue(cameraName, out var target))
        {
            SetCamera(target);
        }
        else
        {
            Debug.LogWarning($"[Camera] 카메라 '{cameraName}'을 찾을 수 없습니다.");
        }
    }
}
