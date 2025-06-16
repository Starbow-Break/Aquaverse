using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomHandler : NetworkBehaviour
{
    [SerializeField] private ConnectionData _data;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _titleImage;
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _startButtonText;
    [SerializeField] private TextMeshProUGUI _clientStatusText;
    
    [Header("Images")]
    [SerializeField] private Sprite game1_Image;
    [SerializeField] private Sprite game2_Image;
    
    [Networked, Capacity(2)]
    private NetworkDictionary<PlayerRef, bool> _ready =>
        default;
    
    private bool _localPreviewReady;
    private Color yellowColor = new Color(1f, 0.7803922f, 0.09411765f, 1);
    private Color greyColor = new Color(0.5176471f, 0.5176471f, 0.5176471f, 1);

    public override void Spawned()
    {
        _startButton.onClick.AddListener(OnClickMain);
        
        bool isGame1 = Runner.name == "FirstGame";
        
        _titleText.text = isGame1 ? "Roll Together" : "Plat Jump";
        _titleImage.sprite = isGame1 ? game1_Image : game2_Image;

        UpdateButtonUI(force: true);
    }
    
    public override void FixedUpdateNetwork()
    {
        UpdateButtonUI();
    }
    
    private void OnClickMain() {
        if (ConnectionManager.Instance.IsGameHost()) {
            if (!AreAllReady()) return;
            StartGame();
            return;
        }

        _localPreviewReady = !_localPreviewReady;
        _clientStatusText.color = _localPreviewReady ? yellowColor : greyColor;
        UpdateButtonUI(force: true);
        RpcToggleReady();
    }
    
    [Rpc(RpcSources.Proxies, RpcTargets.StateAuthority)]
    private void RpcToggleReady(RpcInfo info = default)
    {
        var player = info.Source;

        bool current = _ready.TryGet(player, out var val) && val;

        _ready.Set(player, !current);
        
        if (HasStateAuthority)
            _clientStatusText.color = !current ? yellowColor : greyColor;
    }

    
    private void StartGame()
    {
        Runner.SessionInfo.IsOpen = false;

        int targetSceneIndex = Runner.name switch
        {
            "FirstGame"  => 3,
            "SecondGame" => 4,
            _ => throw new InvalidOperationException($"Unknown session name: {Runner.SessionInfo.Name}")
        };

        Runner.LoadScene(SceneRef.FromIndex(targetSceneIndex));
    }
    
    private void UpdateButtonUI(bool force = false) {
        bool isHost = ConnectionManager.Instance.IsGameHost();

        bool allReady = AreAllReady();

        string label = isHost ? "Start"
            : (_localPreviewReady ? "Cancel" : "Ready");

        if (force || _startButtonText.text != label)
            _startButtonText.text = label;

        _startButton.interactable = !isHost || allReady;
    }


    private bool AreAllReady()
    {
        if (_ready.Count == 0) return false;

        foreach (var kvp in _ready)
            if (!kvp.Value)
                return false;

        return true;
    }


    public void OnClickExit() =>
        _ = ConnectionManager.Instance.ConnectToRunner(_data);
}
