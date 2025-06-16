using System;

public class Trigger
{
    private bool _value;

    public event Action OnShot;

    public Trigger(bool value = false)
    {
        _value = value;
    }

    public void Ready()
    {
        _value = true;
    }
    
    public void Release()
    {
        _value = false;
    }

    public bool TryShot()
    {
        if (_value)
        {
            Shot();
            return true;
        }
        
        return false;
    }

    private void Shot()
    {
        OnShot?.Invoke();
        _value = false;
    }

    public void Reset()
    {
        _value = false;
        OnShot = null;
    }
}
