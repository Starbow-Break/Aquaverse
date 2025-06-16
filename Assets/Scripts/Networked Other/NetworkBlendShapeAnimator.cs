using UnityEngine;
using System.Collections;
using Fusion;


public class NetworkBlendShapeAnimator : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(UpdateBlendShapeValue))]
    private float _currentBlendShapeValue { get; set; }

    private SkinnedMeshRenderer skinnedMeshRenderer;
    public int blendShapeIndex = 0;
    public float maxBlendShapeValue = 100f;
    public float animationSpeed = 1f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        
        if (skinnedMeshRenderer != null)
        {
            StartCoroutine(AnimateBlendShape());
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found on the GameObject or its children.");
        }
    }

    private IEnumerator AnimateBlendShape()
    {
        while (true)
        {
            yield return AnimateToValue(maxBlendShapeValue);
            yield return AnimateToValue(0f);
        }
    }

    private IEnumerator AnimateToValue(float targetValue)
    {
        float elapsedTime = 0f;
        float initialBlendShapeValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
        float duration = 1f / animationSpeed;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = animationCurve.Evaluate(normalizedTime);
            _currentBlendShapeValue = Mathf.Lerp(initialBlendShapeValue, targetValue, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, targetValue);
    }

    private void UpdateBlendShapeValue()
    {
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, _currentBlendShapeValue);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (animationCurve == null)
        {
            animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
    }
#endif
}

