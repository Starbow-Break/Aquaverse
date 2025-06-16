using Fusion;
using UnityEngine;

public class NetworkOscillateRotation : NetworkBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationAngle = 45f;
    public float duration = 2f;
    public bool useRandomDelay = false; // Toggle random delay
    public float maxRandomDelay = 1f; // Maximum random delay

    private Quaternion startRotation;
    private float timeElapsed = 0f;
    private bool isReversing = false;
    private float randomDelay = 0f;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            startRotation = transform.rotation;

            if (useRandomDelay)
            {
                randomDelay = Random.Range(0f, maxRandomDelay);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (timeElapsed < randomDelay)
            {
                timeElapsed += Runner.DeltaTime;
                return;
            }

            float progress = (timeElapsed - randomDelay) / (duration / 2f);
            progress = Mathf.Clamp01(progress);

            progress = Curves.EaseInOut(progress);

            float currentAngle = rotationAngle * (isReversing ? (1 - progress) : progress);
            Quaternion currentRotation = startRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);

            transform.rotation = currentRotation;

            timeElapsed += Runner.DeltaTime;

            if (timeElapsed >= duration / 2f + randomDelay)
            {
                timeElapsed = randomDelay;
                isReversing = !isReversing;
            }
        }
    }
}

