using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Ball : Grabbable
{
    [SerializeField] private Transform _model;
    [SerializeField] private float _castRadius;
    [SerializeField] private float _castDistance;
    
    private Vector3 _lastPosition;
    private bool _roll;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override bool IsCollide(Vector3 direction)
    {
        var hits = Physics.SphereCastAll(transform.position, _castRadius, direction, _castDistance);
        
        float minAngle = 180f;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject) continue;
            minAngle = Mathf.Min(minAngle, Vector3.Angle(Vector3.up, hit.normal));
        }
        
        return minAngle < 120f;
    }
    
    public override bool IsInteractable(GrabInteractor interactor)
    {
        var playerMovement = interactor.GetComponent<PlayerMovement>();
        var playerData = playerMovement.PlayerData;
        return playerData.PlayerScale == EPlayerScale.Big;
    }
    
    public override bool TryInteract(GrabInteractor interactor)
    {
        if (HasStateAuthority && IsInteractable(interactor))
        {
            Interact(interactor);
            return true;
        }

        return false;
    }
    
    public override void Interact(GrabInteractor interactor)
    {
        base.Interact(interactor);
        _lastPosition = transform.position;
    }
    
    public override void FinishInteract(GrabInteractor interactor)
    {
        if (HasStateAuthority)
        {
            base.FinishInteract(interactor);
            _roll = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        if (currentInteractor != null && HasStateAuthority)
        {
            if (!_roll)
            {
                _roll = true;
                return;
            }

            Roll();
        }
        
        _lastPosition =  transform.position;
    }
    
    private void Roll()
    {
        Vector3 velocity = (transform.position - _lastPosition) / Runner.DeltaTime;
            
        if (velocity.magnitude <= 0f) return;
            
        Vector3 roundAxis = Vector3.Cross(Vector3.up, velocity);
        
        float angularSpeed = 360f / _model.lossyScale.x * velocity.magnitude;
        transform.RotateAround(_model.position, roundAxis, angularSpeed * Runner.DeltaTime);
    }
}