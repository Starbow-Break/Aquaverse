using UnityEngine;

public static class Constant
{
    #region Animation
    /// <summary>
    /// 애니메이션 해시값
    /// </summary>
    public static int SpeedHash = Animator.StringToHash("speed");
    public static int SpeedYHash = Animator.StringToHash("speedY");
    public static int IsGroundedHash = Animator.StringToHash("isGrounded");
    public static int JumpHash = Animator.StringToHash("jump");
    public static int IsWallHash = Animator.StringToHash("isWall");
    public static int IsGrabbingHash = Animator.StringToHash("isGrabbing");
    #endregion
    
    #region GameSpawnPosition
    /// <summary>
    /// 캐릭터 스폰 포인트 및 회전 각도
    /// </summary>
    public static Vector3 spawnPoint1 = new Vector3(0f, 2.1f, 0.5f);
    public static Vector3 spawnPoint2 = new Vector3(0f, 2.1f, -0.8f);
    public static Quaternion spawnRotation = Quaternion.Euler(0, 90f, 0);
    #endregion
}
