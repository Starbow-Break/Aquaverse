using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }
    
    public GateUI GateUI;
    public GameObject EscUI;
    public ClearUI ClearUI;
    public GameObject PauseUI;
    public GameObject SettingUI;

    public bool isActive { get; set; }
    public List<PlayerEsc> uiActivePlayers = new();
    public int UIActiveCount => uiActivePlayers.Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MouseEnable();
    }

    public void MouseEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        isActive = true;
    }
    
    public void MouseDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isActive = false;
    }
    
    public void ClearInterface()
    {
        if (UIScreen.activeScreen == null) return;
        UIScreen.activeScreen.Defocus();
    }
}
