using System;
using Fusion;
using Fusion.Addons.KCC;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkBool _canMove { get; set; } = true;
    private NetworkBool _canJump { get; set; } = true;
    [SerializeField] private GameObject freeLookPrefab;
    [SerializeField] private Transform cameraPivot;

    private GameObject _vcamInstance;
    private Camera _cam;

    private CinemachineOrbitalFollow _myOrbitalFollow;
    private CinemachineInputAxisController _myInputAxisController;

    private NetworkAnimatorController _animatorController;

    private KCC _cc;
    private Vector3 _dir;
    private float camSpeed = 20f;

    private float smoothYaw;
    private float smoothPitch;
    private float smoothSpeed = 15f;

    [Networked] public float NetworkYaw { get; set; }
    [Networked] public float NetworkPitch { get; set; }

    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
    public InteractionUIUpdater InteractionUIUpdater { get; private set; }

    private GrabInteractor _grabInteractor;

    [Networked] private TickTimer _moveTimer { get; set; }
    [Networked] private TickTimer _jumpTimer { get; set; }
    
    #region Animation Values
    [Networked, OnChangedRender(nameof(UpdateSpeedAnim))] private float speed { get; set; }
    [Networked, OnChangedRender(nameof(UpdateSpeedAnim))] private float speedY { get; set; }
    #endregion

    private Action _playJumpTimer;

    private bool isUIActiveInGame => Runner.GameMode != GameMode.Shared 
                               && (InterfaceManager.Instance.isActive || InterfaceManager.Instance.UIActiveCount >= 1);
    private bool isLocalUIActive => Runner.GameMode == GameMode.Shared 
                                    && (InterfaceManager.Instance.isActive || ChatManager.Instance.isInputFocused);
    
    private bool isUIActive => isUIActiveInGame || isLocalUIActive;

    public override void Spawned()
    {
        _cc = GetComponent<KCC>();
        _grabInteractor = GetComponent<GrabInteractor>();
        _animatorController = GetComponent<NetworkAnimatorController>();

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        int playerId = Object.InputAuthority.PlayerId;
        _playerData = new PlayerData(playerId);

        _playJumpTimer = () => _jumpTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
        _playerData.JumpTrigger.OnShot += _playJumpTimer;

        if (Runner.GameMode is GameMode.Shared)
        {
            if (HasStateAuthority)
            {
                _vcamInstance = Instantiate(freeLookPrefab);
                var freeLook = _vcamInstance.GetComponent<CinemachineCamera>();

                freeLook.Follow = cameraPivot;
                freeLook.LookAt = cameraPivot;
                _myInputAxisController = freeLook.GetComponent<CinemachineInputAxisController>();
                
                TryBindMyInteractionUIUpdaterInShared();
            }
        }

        else if (Runner.GameMode is GameMode.Client or GameMode.Host)
        {
            if (currentScene == "RoomScene") return;
            if (HasInputAuthority)
            {
                TryBindMyCamera();
                TryBindMyInteractionUIUpdater();
            }
            else
            {
                TryBindOtherCamera();
                TryBindOtherInteractionUIUpdater();
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _playerData.JumpTrigger.OnShot -= _playJumpTimer;
    }

    public override void FixedUpdateNetwork()
    {
        if (IsProxy) return;
        
        _playerData.ReleaseAllTrigger();
        
        UpdateMoveCD();
        UpdateJumpCD();
        
        if (!_canMove) return;
        
        if (GetInput<MyNetworkInput>(out var input))
        {
            Vector3 camForward = _vcamInstance.transform.forward;
            Vector3 camRight = _vcamInstance.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            
            _dir = Vector3.zero;

            if (input.IsDown(MyNetworkInput.BUTTON_FORWARD)) _dir += camForward;
            else if (input.IsDown(MyNetworkInput.BUTTON_BACKWARD)) _dir -= camForward;
            if (input.IsDown(MyNetworkInput.BUTTON_RIGHT)) _dir += camRight;
            else if (input.IsDown(MyNetworkInput.BUTTON_LEFT)) _dir -= camRight;

            if (_grabInteractor?.Grabbable != null && _grabInteractor?.Grabbable?.IsCollide(_dir) == true)
            {
                _dir = Vector3.zero;
            }
            
            if (isUIActive)
            {
                if (_myInputAxisController != null) _myInputAxisController.enabled = false;
                _dir = Vector3.zero;
                input.LookYaw = 0;
                input.LookPitch = 0;
            }
            else
            {
                if (_myInputAxisController != null) _myInputAxisController.enabled = true;
            }
                
            NetworkYaw -= input.LookYaw * camSpeed;
            NetworkPitch += input.LookPitch * camSpeed;
            
            NetworkYaw = Mathf.Clamp(NetworkYaw, -10f, 45f);
            
            _cc.SetInputDirection(_dir.normalized);

            if (_grabInteractor?.Grabbable == null && _dir.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(_dir);
                _cc.SetLookRotation(rot);
            }

            if (_cc.Data.IsGrounded)
            {
                _playerData.Running = input.IsDown(MyNetworkInput.BUTTON_RUN);
            }

            if (_grabInteractor?.Grabbable == null && !isUIActive)
            {
                if (_canJump && input.IsDown(MyNetworkInput.BUTTON_JUMP))
                {
                    _playerData.JumpTrigger.Ready();
                }
            
                if (input.IsDown(MyNetworkInput.BUTTON_LEFTCLICK))
                {
                    _playerData.SmallerTrigger.Ready();
                    _cc.ExecuteStage<ISetScale>();
                }
                else if (input.IsDown(MyNetworkInput.BUTTON_RIGHTCLICK))
                {
                    _playerData.BiggerTrigger.Ready();
                    _cc.ExecuteStage<ISetScale>();
                }
            }
        }
    }
    
    public override void Render()
    {
        if (HasStateAuthority)
        {
            UpdateAnimator();    
        }
        
        if (_myOrbitalFollow == null) return;
        smoothYaw = Mathf.Lerp(smoothYaw, NetworkYaw, Time.deltaTime * smoothSpeed);
        smoothPitch = Mathf.Lerp(smoothPitch, NetworkPitch, Time.deltaTime * smoothSpeed);

        _myOrbitalFollow.VerticalAxis.Value = smoothYaw;
        _myOrbitalFollow.HorizontalAxis.Value = smoothPitch;
    }
    
    private void UpdateAnimator()
    {
        Vector3 moveSpeed = _cc.Data.RealVelocity;
        speedY = moveSpeed.y; 
        moveSpeed = new Vector3(moveSpeed.x, 0, moveSpeed.z);
        
        var environmentProcessor = _cc.GetProcessor<EnvironmentProcessor>();
        if (!environmentProcessor) return;

        float normalSpeed = environmentProcessor.KinematicSpeed;
        var runProcessor = _cc.GetProcessor<RunProcessor>();
        float maxMoveSpeed = normalSpeed * (runProcessor != null ? runProcessor.RunMultiplier : 1);
        
        speed = moveSpeed.magnitude / maxMoveSpeed;
    }

    private void UpdateSpeedAnim()
    {
        _animatorController.Animator.SetFloat(Constant.SpeedHash, speed);
        _animatorController.Animator.SetFloat(Constant.SpeedYHash, speedY);
    }
    
    public void TryBindMyCamera()
    {
        if (HasInputAuthority)
        {
            var MyChannel = (int)Object.InputAuthority.PlayerId;

            var camSet = CameraHolder.Instance.GetCameraSet(MyChannel);

            if (camSet != null)
            {
                camSet.Camera.Follow = cameraPivot;
                camSet.Camera.LookAt = cameraPivot;

                _vcamInstance = camSet.Camera.gameObject;
                _cam = camSet.MainCameraObj.GetComponent<Camera>();

                _myOrbitalFollow = _vcamInstance.GetComponent<CinemachineOrbitalFollow>();

                var axisController = _vcamInstance.GetComponent<CinemachineInputAxisController>();
                if (axisController != null)
                {
                    axisController.enabled = false;
                }
            }
        }
    }
    
    public void TryBindOtherCamera()
    {
        if(!HasInputAuthority)
        {
            var MyChannel = (int)Object.InputAuthority.PlayerId == 1 ? 1 : 2;
            var camSet = CameraHolder.Instance.GetCameraSet(MyChannel);
            if (camSet != null)
            {
                camSet.Camera.Follow = cameraPivot;
                camSet.Camera.LookAt = cameraPivot;
                    
                _vcamInstance = camSet.Camera.gameObject;
                _cam = camSet.MainCameraObj.GetComponent<Camera>();
                    
                _myOrbitalFollow = _vcamInstance.GetComponent<CinemachineOrbitalFollow>();
                    
                var axisController = _vcamInstance.GetComponent<CinemachineInputAxisController>();
                if (axisController != null)
                {
                    axisController.enabled = false;
                }
            }
        }
    }

    public void TryBindMyInteractionUIUpdaterInShared()
    {
        var interactionUiUpdater = InteractionUIHolder.Instance.GetInteractionUIUpdater(1);
        if (interactionUiUpdater != null)
        {
            InteractionUIUpdater = interactionUiUpdater;
            InteractionUIUpdater.SetCamera(Camera.main);
        }
    }

    public void TryBindMyInteractionUIUpdater()
    {
        if (HasInputAuthority)
        { 
            var updaterIndex = Object.InputAuthority.PlayerId;
            
            var interactionUiUpdater = InteractionUIHolder.Instance.GetInteractionUIUpdater(updaterIndex);
            if (interactionUiUpdater != null)
            {
                InteractionUIUpdater = interactionUiUpdater;
                InteractionUIUpdater.SetCamera(_cam);
            }
        }
    }
    
    public void TryBindOtherInteractionUIUpdater()
    {
        if(!HasInputAuthority)
        {
            var updaterIndex = (int)Object.InputAuthority.PlayerId == 1 ? 1 : 2;
            var interactionUiUpdater = InteractionUIHolder.Instance.GetInteractionUIUpdater(updaterIndex);
            if (interactionUiUpdater != null)
            {
                InteractionUIUpdater = interactionUiUpdater;
                InteractionUIUpdater.SetCamera(_cam);
            }
        }
    }
    
    public void DisableMovementForSeconds(float time)
    {
        _moveTimer = TickTimer.CreateFromSeconds(Runner, time);
    }
    
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
    
    private void UpdateMoveCD()
    {
        _canMove = _moveTimer.ExpiredOrNotRunning(Runner);
    }
    
    private void UpdateJumpCD()
    {
        _canJump = _jumpTimer.ExpiredOrNotRunning(Runner);
    }
}