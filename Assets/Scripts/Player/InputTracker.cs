using System;
using System.Collections.Generic;
using UnityEngine;

public class InputTracker : MonoBehaviour
{
    Dictionary<KeyCode, bool> IsKeyDown = new();
    Dictionary<KeyCode, bool> IsKeyUp = new();

    public static InputTracker Instance;

    private List<KeyCode> _keys = new();
    public float LookYaw { get; private set; }
    public float LookPitch { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void Start()
    {
        string[] keyNames = Enum.GetNames(typeof(KeyCode));
        foreach (var keyName in keyNames)
        {
            _keys.Add(Enum.Parse<KeyCode>(keyName));
        }

        foreach (var key in _keys)
        {
            IsKeyDown.TryAdd(key, false);
            IsKeyUp.TryAdd(key, false);
        }
    }
    
    private void Update()
    {
        LookYaw = Input.GetAxis("Mouse Y");
        LookPitch = Input.GetAxis("Mouse X");
        
        foreach (var key in _keys)
        {
            if (Input.GetKeyDown(key))
            {
                IsKeyDown[key] = true;
            }
        }
        
        foreach (var key in _keys)
        {
            if (Input.GetKeyUp(key))
            {
                IsKeyUp[key] = true;
            }
        }
    }

    public bool GetKeyDown(KeyCode keyCode)
    {
        if (IsKeyDown.TryGetValue(keyCode, out var result))
        {
            return result;
        }

        return false;
    }
    
    public bool GetKeyUp(KeyCode keyCode)
    {
        if (IsKeyUp.TryGetValue(keyCode, out var result))
        {
            return result;
        }

        return false;
    }

    public void Clear()
    {
        foreach (var key in _keys)
        {
            IsKeyDown[key] = false;
        }

        foreach (var key in _keys)
        {
            IsKeyUp[key] = false;
        }
    }
}
