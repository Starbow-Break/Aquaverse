using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class SaveLoadPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action onYesCallback;
    private Func<UniTask> onYesAsync;
    private Action onNoCallback;

    public void Show(string message, Action onYes, Action onNo = null)
    {
        root.SetActive(true);
        messageText.text = message;
        onYesCallback = onYes;
        onYesAsync = null;
        onNoCallback = onNo;
    }
    
    public void Show(string message, Func<UniTask> onYes, Action onNo = null)
    {
        root.SetActive(true);
        messageText.text = message;
        onYesCallback = null;
        onYesAsync = onYes;
        onNoCallback = onNo;
    }

    private void Awake()
    {
        yesButton.onClick.AddListener(() => _ = HandleYesClickedAsync());

        noButton.onClick.AddListener(() =>
        {
            onNoCallback?.Invoke();
            root.SetActive(false);
        });
        
        root.SetActive(false); // 시작 시 꺼둠
    }
    
    private async UniTaskVoid HandleYesClickedAsync()
    {
        if (onYesAsync != null)
        {
            await onYesAsync();
        }
        else
        {
            onYesCallback?.Invoke();
        }

        root.SetActive(false);
    }
}