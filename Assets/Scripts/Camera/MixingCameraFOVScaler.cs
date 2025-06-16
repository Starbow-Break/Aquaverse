using Unity.Cinemachine;
using UnityEngine;

public class MixingCameraFOVScaler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camA;
    [SerializeField] private CinemachineCamera camB;
    
    private float minZOffset = -60f;
    private float maxZOffset = -240f;

    private float xThreshold = 10f;
    private float yThreshold = 5f;
    
    private float maxX = 60f;
    private float maxY = 30f;

    private CinemachineFollow aFollow;
    private CinemachineFollow bFollow;

    private void Awake()
    {
        aFollow = camA.GetComponent<CinemachineFollow>();
        bFollow = camB.GetComponent<CinemachineFollow>();
    }
    
    void LateUpdate()
    {
        // 거리 계산 (플레이어 or 카메라 기준)
        Vector3 posA = camA.transform.position;
        Vector3 posB = camB.transform.position;

        float dx = Mathf.Abs(posA.x - posB.x);
        float dy = Mathf.Abs(posA.y - posB.y);

        float xRatio = Mathf.Clamp01((dx - xThreshold) / (maxX - xThreshold));
        float yRatio = Mathf.Clamp01((dy - yThreshold) / (maxY - yThreshold));
        
        float t = Mathf.Max(xRatio, yRatio);
        float targetZ = Mathf.Lerp(minZOffset, maxZOffset, t);
        
        // Debug.Log($"dx: {dx}, dy: {dy}, weightedDistance: {weightedDistance}, targetFOV: {targetFOV}");

        aFollow.FollowOffset.z = Mathf.Lerp(aFollow.FollowOffset.z, targetZ, Time.deltaTime * 5f);
        bFollow.FollowOffset.z = Mathf.Lerp(bFollow.FollowOffset.z, targetZ, Time.deltaTime * 5f);
    }
}
