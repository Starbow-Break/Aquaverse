using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class SwitchGroup : NetworkBehaviour
{
    [SerializeField] private List<Switch> _switches;

    public bool success { get; private set; } = false;
    public UnityEvent OnSuccess;

    public void Start()
    {
        OnSuccess.AddListener(() => Debug.Log("Lets Gooooooooooo!!!!!!!!!"));
    }

    public override void Spawned()
    {
        foreach (var s in _switches)
        {
            s.SetSwitchGroup(this);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (success) return;

        if (CheckSuccess())
        {
            ActSuccess();
        }
    }

    private void ActSuccess()
    {
        success = true;
        OnSuccess?.Invoke();
    }

    private bool CheckSuccess()
    {
        bool result = true;
        
        foreach(var switchEle in _switches)
        {
            if (!switchEle.isPressed)
            {
                result = false;
                break;
            }
        }

        return result;
    }
}
