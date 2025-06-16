using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour {
    public static UIScreen activeScreen;
    
    [HideInInspector] public UIScreen previousScreen = null;
    
    public bool isModal = false;
    
    CanvasGroup _group = null;
    public CanvasGroup Group
    {
        get
        {
            if (_group) return _group;
            return _group = GetComponent<CanvasGroup>();
        }
    }
    
    public void Defocus()
    {
        gameObject.SetActive(false);
        InterfaceManager.Instance.MouseDisable();
        activeScreen = null;
    }
    
    public virtual void Focus()
    {
        if (activeScreen == this && activeScreen.gameObject.activeInHierarchy)
            return;
        
        Group.interactable = true;
        gameObject.SetActive(true);
        InterfaceManager.Instance.MouseEnable();
        activeScreen = this;
    }
}