using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerInteractor: Interactor
{
    [Networked] private ATriggerInteractable HitInteractable { get; set; }
    [Networked, OnChangedRender(nameof(OnChangeTriggerInteractable))] 
    private ATriggerInteractable TriggerInteractable { get; set; }
    private PlayerMovement _playerMovement;
    
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (HitInteractable != null && HitInteractable.IsInteractable(this))
        {
            var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
            interactionUIUpdater?.SetActiveTriggerInteractionUI(true);
            interactionUIUpdater?.SetTriggerInteractionUIPositionWorld(HitInteractable.transform.position);    
        }
        else
        {
            var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
            interactionUIUpdater?.SetActiveTriggerInteractionUI(false);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (_playerMovement == null) return;
        
        if (HasStateAuthority && GetInput(out MyNetworkInput data))
        {
            if (data.IsDown(MyNetworkInput.BUTTON_INTERACT))
            {
                var result = HitInteractable?.TryInteract(this) ?? false;

                if (result)
                {
                    if (HitInteractable.isClosable)
                    {
                        TriggerInteractable?.FinishInteract(this);

                        TriggerInteractable = HitInteractable;
                        Debug.Log("*");
                        var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
                        Debug.Log(interactionUIUpdater);
                        interactionUIUpdater?.SetActiveTriggerEndInteractionUI(true);
                    }
                }
            }

            if (data.IsDown(MyNetworkInput.BUTTON_END_INTERACT))
            {
                TriggerInteractable?.FinishInteract(this);

                if (TriggerInteractable != null)
                {
                    TriggerInteractable = null;
                    var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
                    interactionUIUpdater?.SetActiveTriggerEndInteractionUI(false);
                }
            }
        }
    }

    private void OnChangeTriggerInteractable()
    {
        var interactionUIUpdater = _playerMovement.InteractionUIUpdater;
        interactionUIUpdater?.SetActiveGrabEndInteractionUI(TriggerInteractable != null);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;
        
        ATriggerInteractable interactable = other.GetComponent<ATriggerInteractable>();
        interactable ??= other.GetComponentInParent<ATriggerInteractable>();
        
        if (interactable == null) return;
        
        if (HitInteractable == null)
        {
            HitInteractable = interactable;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority) return;
        
        ATriggerInteractable interactable = other.GetComponent<ATriggerInteractable>();
        interactable ??= other.GetComponentInParent<ATriggerInteractable>();
    
        if (interactable == null) return;
        
        if (HitInteractable == interactable)
        {
            HitInteractable = null;
        }
    }
}
