using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUI : UIScreen
{
    public TMP_InputField inputField;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect sv;
    
    private Coroutine _defocusCoroutine;

    private void Awake()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void AddMessage(string text, bool isLocal)
    {
        var go = Instantiate(messagePrefab, content);
        var tmp = go.GetComponentInChildren<TMP_Text>();
        tmp.text = text;
        
        Canvas.ForceUpdateCanvases();
        Focus();
        InterfaceManager.Instance.MouseDisable();
        
        bool isRecent = sv.verticalNormalizedPosition < 3f;
        if (isRecent)
        {
            sv.verticalNormalizedPosition = 0f;
        }


        if (ChatManager.Instance.isInputFocused)
            return;
        
        StopDefocusCoroutine();
        _defocusCoroutine = StartCoroutine(DefocusProcess());
    }
    
    public void StopDefocusCoroutine()
    {
        if (_defocusCoroutine != null)
        {
            StopCoroutine(_defocusCoroutine);
            _defocusCoroutine = null;
        }
    }
    
    public void DefocusImmediate()
    {
        if (_defocusCoroutine != null)
        {
            StopCoroutine(_defocusCoroutine);
            _defocusCoroutine = null;
        }
        
        SetBackgroundAlpha(0f);
        Defocus();
    }
    
    private IEnumerator DefocusProcess()
    {
        yield return new WaitForSeconds(5f);
        SetBackgroundAlpha(0f);
        Defocus();
    }
    
    public void SetBackgroundAlpha(float alpha)
    {
        var image = sv.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}