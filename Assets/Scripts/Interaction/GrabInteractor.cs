using System.Collections;
using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class GrabInteractor : Interactor
{
    [SerializeField] private NetworkAnimatorController _animatorController;
    
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private float _distance = 2f;
    [SerializeField] private Color _gizmoColor = Color.red;
    
    private PlayerMovement _playerMovement;
    [Networked] private Grabbable hitGrabbable { get; set; }
    [Networked] private Vector3 hitPosition { get; set; }
    
    public Transform GrabPoint => _grabPoint;
    [Networked, OnChangedRender(nameof(OnChangeGrabbable))] 
    public Grabbable Grabbable { get; set; }
    
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        if (Grabbable != null)
        {
            _playerMovement.InteractionUIUpdater?.SetActiveGrabEndInteractionUI(true);
            _playerMovement.InteractionUIUpdater?.SetActiveGrabInteractionUI(false);
        }
        else
        {
            _playerMovement.InteractionUIUpdater?.SetActiveGrabEndInteractionUI(false);
            if (hitGrabbable != null)
            {
                _playerMovement.InteractionUIUpdater?.SetActiveGrabInteractionUI(true);
                _playerMovement.InteractionUIUpdater?.SetGrabInteractionUIPositionWorld(hitPosition);
            }
            else
            {
                _playerMovement.InteractionUIUpdater?.SetActiveGrabInteractionUI(false);
            }
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (_playerMovement == null || !HasStateAuthority) return;

        if (Grabbable == null)
        {
            if (Physics.Raycast(_grabPoint.position, _grabPoint.forward, out RaycastHit hit, _distance))
            {
                hitGrabbable = hit.collider.GetComponent<Grabbable>();
                hitPosition = hit.point;
                if (hitGrabbable == null || !hitGrabbable.IsInteractable(this))
                {
                    hitGrabbable = null;
                }
            }
            else
            {
                hitGrabbable = null;
            }
        }
        
        if (GetInput(out MyNetworkInput data) && HasStateAuthority)
        {
            if (hitGrabbable != null)
            {
                if (data.IsDown(MyNetworkInput.BUTTON_INTERACT))
                {
                    bool result = hitGrabbable.TryInteract(this);
                    if (result)
                    {
                        Grabbable = hitGrabbable;
                    }
                }
            }

            if (data.IsDown(MyNetworkInput.BUTTON_END_INTERACT))
            {
                if (Grabbable == null) return;
            
                Grabbable.FinishInteract(this);
                Grabbable = null;
            }
        }
    }

    private void OnChangeGrabbable()
    {
        _animatorController.RPC_SetBool(Constant.IsGrabbingHash, Grabbable);
        var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
        interactionUIUpdater.SetActiveGrabEndInteractionUI(Grabbable);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawLine(_grabPoint.position, _grabPoint.position + _distance * _grabPoint.forward);
    }
}
