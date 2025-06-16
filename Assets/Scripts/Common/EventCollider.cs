using System;
using UnityEngine;

public class EventCollider : MonoBehaviour
{
    public event Action<Collider> TriggerEnter;
    public event Action<Collider> TriggerExit;
    public event Action<Collision> CollisionEnter;
    public event Action<Collision> CollisionExit;
    
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        CollisionEnter?.Invoke(other);
    }
    
    private void OnCollisionExit(Collision other)
    {
        CollisionExit?.Invoke(other);
    }
}
