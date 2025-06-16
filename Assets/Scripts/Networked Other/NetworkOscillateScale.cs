using Fusion;
using UnityEngine;

public class NetworkOscillateScale : NetworkBehaviour
{
    public Vector3 scaleAxis = Vector3.one;
    public float scaleFactor = 2f;
    public float duration = 2f;
    public bool useRandomDelay = false; // Toggle random delay
    public float maxRandomDelay = 1f; // Maximum random delay

    private Vector3 _startScale;
    private float _timeElapsed = 0f;
    private bool _isReversing = false;
    private float _randomDelay = 0f;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _startScale = transform.localScale;

            if (useRandomDelay)
            {
                _randomDelay = Random.Range(0f, maxRandomDelay);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority) {
            if (_timeElapsed < _randomDelay)
            {
                _timeElapsed += Runner.DeltaTime;
                return;
            }

            float progress = (_timeElapsed - _randomDelay) / (duration / 2f);
            progress = Mathf.Clamp01(progress);

            progress = Curves.EaseInOut(progress);

            Vector3 currentScale = _startScale +
                                   scaleAxis.normalized * (scaleFactor - 1) * (_isReversing ? (1 - progress) : progress);

            transform.localScale = currentScale;

            _timeElapsed += Runner.DeltaTime;

            if (_timeElapsed >= duration / 2f + _randomDelay)
            {
                _timeElapsed = _randomDelay;
                _isReversing = !_isReversing;
            }
        }
    }
}

