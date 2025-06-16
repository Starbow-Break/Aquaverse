using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PlayerNetworkApplier : NetworkBehaviour
{
    [Networked, Capacity(512)]
    private NetworkString<_512> _receivedJson { get; set; }
    
    private PlayerCustomizationApplier _applier;
    private bool _applied = false;

    public override void Spawned()
    {
        _applier = GetComponent<PlayerCustomizationApplier>();
        if (Object.HasInputAuthority)
        {
            DelaySendCustomization().Forget();
        }
    }
    
    private async UniTaskVoid DelaySendCustomization()
    {
        await UniTask.WaitUntil(() => Runner != null);
        string json = CustomizationManager.Instance.LoadRawAsJson();
        
        if (Runner.GameMode == GameMode.Shared && HasStateAuthority)
        {
            // Shared 모드: 직접 할당 가능
            _receivedJson = json;
            ApplyCustomization();
        }
        
        else if(Object.HasInputAuthority)
        {
            RPC_SendCustomizationToServer(json);
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendCustomizationToServer(string json)
    {
        _receivedJson = json;
        ApplyCustomization();
    }
    
    public override void Render()
    {
        if (!_applied && !string.IsNullOrEmpty(_receivedJson.ToString()))
        {
            ApplyCustomization();
            _applied = true;
        }
    }
    
    public void ApplyCustomization()
    {
        if (_applier == null) return;
        var data = CustomizationManager.Instance.ParseJsonToCustomizationData(_receivedJson.ToString());
        _applier.ApplyCustomization(data);
    }
}
