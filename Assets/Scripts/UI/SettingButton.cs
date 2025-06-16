using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingButton : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    
    private void OnEnable()
    {
        StartCoroutine(AddListenerSequence());
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private IEnumerator AddListenerSequence()
    {
        yield return new WaitUntil(() => InterfaceManager.Instance.SettingUI != null);
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var settingUI = InterfaceManager.Instance.SettingUI;
        var canvas = settingUI.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = true;    
        }
    }
}

