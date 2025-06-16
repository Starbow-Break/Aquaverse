using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.KCC;

public class GameStarter : NetworkBehaviour {
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private List<KCCProcessorInjectData> _injectProcessors;
    
    private Dictionary<PlayerRef, NetworkObject> _playerAvatars;
    
    public override void Spawned()
    {
        InterfaceManager.Instance.MouseDisable();
        
        if (Object.HasStateAuthority == false) return;
        _playerAvatars = new Dictionary<PlayerRef, NetworkObject>();
    
        int i = 0;
        foreach (var playerRef in Runner.ActivePlayers)
        {
            var spawnPos = (i % 2 == 0) ? Constant.spawnPoint1 : Constant.spawnPoint2;
            var netObj = Runner.Spawn(_playerPrefab, spawnPos, Constant.spawnRotation, playerRef);
            Runner.SetPlayerObject(playerRef, netObj);
            _playerAvatars[playerRef] = netObj;

            i++;
            
            foreach (var kvp in _playerAvatars)
            {
                var playerMovement = kvp.Value.GetComponent<PlayerMovement>();
                playerMovement?.TryBindOtherCamera();
                playerMovement?.TryBindOtherInteractionUIUpdater();

                var kcc = kvp.Value.GetComponent<KCC>();
                foreach (var processorInjectData in _injectProcessors)
                {
                    TryInjectKCCProcessor(kcc, processorInjectData);
                }
            }
        }
    }

    private void TryInjectKCCProcessor(KCC kcc, KCCProcessorInjectData processorInjectData)
    {
        KCCProcessor processor = processorInjectData.Processor;
        KCCProcessorInjectData.EInjectTarget target = processorInjectData.InjectTarget;
        
        if(kcc.Object.InputAuthority.PlayerId == 1 && target.HasFlag(KCCProcessorInjectData.EInjectTarget.Host))
        {
            kcc.AddLocalProcessor(processor);
        }
        else if(kcc.Object.InputAuthority.PlayerId == 2 && target.HasFlag(KCCProcessorInjectData.EInjectTarget.Client))
        {
            kcc.AddLocalProcessor(processor);
        }
    }
}