using UnityEngine;

public static class Curves {
    public static float EaseInOut(float t)
    {
        return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
}
