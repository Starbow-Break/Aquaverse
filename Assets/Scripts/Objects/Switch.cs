using System;
using System.Collections;
using Fusion;
using UnityEngine;

public class Switch : ATriggerInteractable
{
    [Networked, OnChangedRender(nameof(OnChangedIsPressed))] 
    public bool isPressed { get; private set; } = false;

    [SerializeField] private GameObject _switchUiObj;
    [SerializeField] private bool isBig;
    public override bool IsInteractable(TriggerInteractor interactor) => !isPressed;

    private SwitchGroup _switchGroup;
    
    public override void Interact(TriggerInteractor interactor)
    {
        if (isBig)
        {
            var playerMovement = interactor.GetComponent<PlayerMovement>();
            var playerData = playerMovement.PlayerData;
            if (playerData.PlayerScale != EPlayerScale.Big)
            {
                return;
            }
        }
        if (!isPressed && HasStateAuthority)
        {
            isPressed = true;
        }
    }

    public override void FinishInteract(TriggerInteractor interactor)
    {
        if (isPressed && HasStateAuthority)
        {
            isPressed = false;
        }
    }

    public void SetSwitchGroup(SwitchGroup group)
    {
        _switchGroup = group;
    }

    private void OnChangedIsPressed()
    {
        _switchUiObj.SetActive(isPressed);
        
        if (!HasStateAuthority) return;
        
        if (isPressed)
        {
            StartCoroutine(InteractSequence());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator InteractSequence()
    {
        yield return new WaitForSeconds(5f);
        if (_switchGroup?.success == true) yield break;
        isPressed = false;
    }
}