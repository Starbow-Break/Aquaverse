using Fusion;
using TMPro;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance { get; private set; }
    
    [SerializeField] private ChatUI _chatUI;

    private TMP_InputField inputField;
    public bool isInputFocused;
    private string msg;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        inputField = _chatUI.inputField;
        
        inputField.onSelect.AddListener(_ => 
        {
            isInputFocused = true;
        });

        inputField.onDeselect.AddListener(_ => 
        {
            isInputFocused = false;
            inputField.gameObject.SetActive(false);
            inputField.DeactivateInputField();
            _chatUI.DefocusImmediate();
        });
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isInputFocused)
            {
                isInputFocused = true;
                
                _chatUI.Focus();
                _chatUI.StopDefocusCoroutine();
                _chatUI.SetBackgroundAlpha(0.5f);
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
            else
            {
                msg = inputField.text.Trim();

                if (string.IsNullOrEmpty(msg))
                {
                    isInputFocused = false;
                    
                    inputField.gameObject.SetActive(false);
                    inputField.DeactivateInputField();
                    _chatUI.DefocusImmediate();
                    
                    InterfaceManager.Instance.MouseDisable();
                }
                else
                {
                    SendChat(msg);
                    inputField.text = "";
                    inputField.ActivateInputField();
                }
            }
        }
    }
    
    public void SendChat(string message)
    {
        if (string.IsNullOrEmpty(message) || !Runner.IsRunning)
            return;
        
        RpcSendChatMessage(message);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcSendChatMessage(string message, RpcInfo info = default)
    {
        string senderId = info.Source.ToString();
        bool isLocal = info.Source == Runner.LocalPlayer;
        
        _chatUI.AddMessage($"{senderId} {message}", isLocal);
    }
}