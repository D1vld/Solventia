using UnityEngine;

public static class UnityEngine_Extensions
{
    public static Vector2 Set(this Vector2 v, float? x = null, float? y = null)
    {
        if (!x.HasValue && !y.HasValue) return v;
        return new Vector2(x ?? v.x, y ?? v.y);
    }

    public static Vector3 Set(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        if (!x.HasValue && !y.HasValue && !z.HasValue) return v;
        return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }

    public static Color ToAlpha(this Color color, float alpha) => new(color.r, color.g, color.b, alpha);
}

[System.Serializable]
public class AudioWrapper
{
    public AudioClip Clip;
    public float Volume = 1f;
    public int DelayInMs;
}