using System;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public abstract class ATriggerInteractable : NetworkBehaviour, IInteraction<TriggerInteractor>
{
    [field: SerializeField] public bool isClosable { get; private set; } = false;
    
    public virtual bool IsInteractable(TriggerInteractor interactor) => true;

    public virtual bool TryInteract(TriggerInteractor interactor)
    {
        if (IsInteractable(interactor))
        {
            Interact(interactor);
            return true;
        }

        return false;
    }

    public abstract void Interact(TriggerInteractor interactor);

    public abstract void FinishInteract(TriggerInteractor interactor);
}
