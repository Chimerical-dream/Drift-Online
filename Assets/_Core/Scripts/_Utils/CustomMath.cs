using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float tt = t * t;

        float u = 1f - t;
        float uu = u * u;

        Vector3 result = uu * p0;
        result += 2f * u * t * p1;
        result += tt * p2;

        return result;
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    public static float ClampPlusMinus1(float v)
    {
        return Mathf.Clamp(v, -1, 1);
    }

    public static float FindLerpProgress(Quaternion a, Quaternion b, Quaternion result)
    {
        // Normalize the quaternions to avoid issues with scaling
        a = Quaternion.Normalize(a);
        b = Quaternion.Normalize(b);
        result = Quaternion.Normalize(result);

        // Calculate the dot products
        float dotAB = Quaternion.Dot(a, b);
        float dotAR = Quaternion.Dot(a, result);

        // Ensure dot products are in valid range [-1, 1]
        dotAB = Mathf.Clamp(dotAB, -1.0f, 1.0f);
        dotAR = Mathf.Clamp(dotAR, -1.0f, 1.0f);

        // Calculate the angle between a and b
        float angleAB = Mathf.Acos(dotAB);

        // Calculate the angle between a and result
        float angleAR = Mathf.Acos(dotAR);

        // Calculate the interpolation factor t
        float t = angleAR / angleAB;

        return t;
    }

    public static float ApproximateSin(float x)
    {
        return 4 * x * (180 - x) / (40500 - x * (180 - x));
    }

    #region Random

    public static int PlusMinusOne => UnityEngine.Random.Range(0f, 1f) > 0.5f ? -1 : 1;
    public static bool RandomBool => UnityEngine.Random.Range(0f, 1f) > 0.5f;

    public static Vector2 Random(Vector2 a, Vector2 b)
    {
        return new Vector2(UnityEngine.Random.Range(a.x, b.x), UnityEngine.Random.Range(a.y, b.y));
    }

    #endregion

    #region Colors
    public static Color SetHsvV(Color c, float v)
    {
        float h, s, x;

        Color.RGBToHSV(c, out h, out s, out x);
        return Color.HSVToRGB(h, s, v);
    }

    public static Color GetEmissionColor(Color c, float intensity)
    {
        float factor = Mathf.Pow(2, intensity);
        c = new Color(c.r * factor, c.g * factor, c.b * factor);
        return c;
    }
    #endregion
}
