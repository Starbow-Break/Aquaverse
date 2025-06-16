using UnityEngine;

public class InformationBoard: ATriggerInteractable
{
    [SerializeField] private GameObject BoardUiObj;
    
    public override void Interact(TriggerInteractor interactor)
    {
        if (BoardUiObj.activeSelf) return;
        
        BoardUiObj.SetActive(true);
    }

    public override void FinishInteract(TriggerInteractor interactor)
    {
        if (!BoardUiObj.activeSelf) return;
        
        BoardUiObj.SetActive(false);
    }
}
