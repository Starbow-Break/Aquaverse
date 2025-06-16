using UnityEngine;

public interface IInteraction<T> where T : Interactor
{
    public bool IsInteractable(T interactor);
    public bool TryInteract(T interactor);
    public void Interact(T interactor);
    public void FinishInteract(T interactor);
}
