using Fusion;
using UnityEngine;

public class NetworkOscillatePosition : NetworkBehaviour
{
    public Vector3 moveAxis = Vector3.up;
    public float moveDistance = 2f;
    public float duration = 2f;
    public bool useRandomDelay = false; // Toggle random delay
    public float maxRandomDelay = 1f; // Maximum random delay

    private Vector3 _startPosition;
    private float _timeElapsed = 0f;
    private bool _isReversing = false;
    private float _randomDelay = 0f;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _startPosition = transform.position;

            if (useRandomDelay)
            {
                _randomDelay = Random.Range(0f, maxRandomDelay);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (_timeElapsed < _randomDelay)
            {
                _timeElapsed += Runner.DeltaTime;
                return;
            }

            float progress = (_timeElapsed - _randomDelay) / (duration / 2f);
            progress = Mathf.Clamp01(progress);

            progress = Curves.EaseInOut(progress);

            float currentDistance = moveDistance * (_isReversing ? (1 - progress) : progress);
            Vector3 currentPosition = _startPosition + moveAxis.normalized * currentDistance;

            transform.position = currentPosition;

            _timeElapsed += Runner.DeltaTime;

            if (_timeElapsed >= duration / 2f + _randomDelay)
            {
                _timeElapsed = _randomDelay;
                _isReversing = !_isReversing;
            }
        }
    }
}
