using Fusion;
using UnityEngine;

public class Grabbable : NetworkBehaviour, IInteraction<GrabInteractor>
{
    protected Rigidbody _rb;

    protected GrabInteractor currentInteractor = null;
    [SerializeField] protected Vector3 localOffset = Vector3.zero;
    [field: SerializeField] public float SpeedMultiplier { get; private set; } = 0.4f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    public virtual bool IsCollide(Vector3 direction) => false;

    public override void FixedUpdateNetwork()
    {
        if (currentInteractor != null)
        {
            UpdateGrabPosition();
        }
    }

    public virtual bool IsInteractable(GrabInteractor interactor) => true;
    
    public virtual bool TryInteract(GrabInteractor interactor)
    {
        if (IsInteractable(interactor))
        {
            Interact(interactor);
            return true;
        }

        return false;
    }

    public virtual void Interact(GrabInteractor interactor)
    {
        currentInteractor = interactor;
        UpdateGrabPosition();
    }

    public virtual void FinishInteract(GrabInteractor interactor)
    {
        currentInteractor = null;
    }

    private void UpdateGrabPosition()
    {
        Matrix4x4 matrix = currentInteractor.GrabPoint.localToWorldMatrix;
        Vector4 offsetTemp = new Vector4(localOffset.x, localOffset.y, localOffset.z, 1);
        Vector4 posTemp = matrix * offsetTemp;
        Vector3 newPosition = new Vector3(posTemp.x, posTemp.y, posTemp.z) / posTemp.w;
        transform.position = newPosition;
    }
}