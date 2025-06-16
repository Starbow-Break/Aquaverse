using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerScaler : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(ApplyScale))]
    private Vector3 _playerScale { get; set; } = Vector3.one;

    [SerializeField] private Transform _model;
    [SerializeField] private Transform _grabPoint;
    
    private KCC _kcc;
    
    private Vector3 _baseModelLocalPosition;
    private Vector3 _baseModelLocalScale;
    private float _baseRadius;
    private float _baseHeight;

    private void Awake()
    {
        _kcc = GetComponent<KCC>();
        
        _baseModelLocalPosition = _model.localPosition;
        _baseModelLocalScale = _model.localScale;
        _baseRadius = _kcc.Collider.radius;
        _baseHeight = _kcc.Collider.height;
    }

    public void SetScale(Vector3 newScale)
    {
        if (HasStateAuthority)
        {
            _playerScale = newScale;
        }
    }

    private void ApplyScale()
    {
        // Model
        _model.transform.localPosition = new Vector3(
            _baseModelLocalPosition.x * _playerScale.x,
            _baseModelLocalPosition.y * _playerScale.y,
            _baseModelLocalPosition.z * _playerScale.z
        );
        _model.transform.localScale = new Vector3(
            _baseModelLocalScale.x * _playerScale.x,
            _baseModelLocalScale.y * _playerScale.y,
            _baseModelLocalScale.z * _playerScale.z
        );
        
        // KCC
        _kcc.SetRadius(_baseRadius * Mathf.Max(_playerScale.x, _playerScale.z));
        _kcc.SetHeight(_baseHeight * _playerScale.y);
    }
}
