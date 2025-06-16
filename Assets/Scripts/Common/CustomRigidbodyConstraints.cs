using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomRigidbodyConstraints : NetworkBehaviour
{
    [Header("Coordinate")]
    [SerializeField] private Transform _coord;
    
    [Header("Constranits Option")]
    [SerializeField] private bool X;
    [SerializeField] private bool Y;
    [SerializeField] private bool Z;

    private Matrix4x4 trs;
    private Vector3 startNewCoordPos;

    public override void Spawned()
    {
        trs = Matrix4x4.TRS(_coord.position, _coord.rotation, Vector3.one);
        startNewCoordPos = GetPositionWorldToCustom(transform.position);
    }
    
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 posNewCoord = GetPositionWorldToCustom(transform.position);

            if (X) posNewCoord.x = startNewCoordPos.x;
            if (Y) posNewCoord.y = startNewCoordPos.y;
            if (Z) posNewCoord.z = startNewCoordPos.z;
        
            transform.position = GetPositionCustomToWorld(posNewCoord);
        }
    }

    Vector3 GetPositionWorldToCustom(Vector3 worldPos)
    {
        Vector4 worldPosTemp = new(worldPos.x, worldPos.y, worldPos.z, 1);
        Vector4 result = trs.inverse * worldPosTemp;
        result /= result.w;
        return new(result.x, result.y, result.z);
    }
    
    Vector3 GetPositionCustomToWorld(Vector3 customPos)
    {
        Vector4 customPosTemp = new(customPos.x, customPos.y, customPos.z, 1);
        Vector4 result = trs * customPosTemp;
        result /= result.w;
        return new(result.x, result.y, result.z);
    }
}
