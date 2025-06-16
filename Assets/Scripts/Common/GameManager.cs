using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnChangeRemainStar))]
    private int _remainStar { get; set; }
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Star[] _stars = FindObjectsByType<Star>(FindObjectsSortMode.None);
            _remainStar = _stars.Length;
            
            foreach (var star in _stars)
            {
                star.OnGet += OnGetStar;
            }
        }
        
        Debug.Log($"Star count: {_remainStar}");
    }

    private void OnChangeRemainStar()
    {
        if (_remainStar <= 0)
        {
            GameClear();
        }
    }

    private void OnGetStar(GameObject getter)
    {
        _remainStar--;
        Debug.Log($"Star count: {_remainStar}");
    }
    
    private void GameClear()
    {
        RPC_ShowClearUI();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_ShowClearUI()
    {
        RPC_DoShowClearUI();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_DoShowClearUI()
    {
        InterfaceManager.Instance.ClearUI.Focus();
    }
}
