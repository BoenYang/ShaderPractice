using UnityEngine;

public class Kernels
{

    public static float Poly6(float sqrDistance, float coef, float smoothRadius) {
        float hSqr = smoothRadius * smoothRadius;
        if (hSqr < sqrDistance) {
            return 0;
        }
        return coef * Mathf.Pow(hSqr - sqrDistance,3);
    }

    public static Vector2 GradientSpiky(Vector2 r, float coef, float smoothRaidus)
    {
        float distance = r.magnitude;
        if (smoothRaidus < distance) {
            return Vector2.zero;
        }
        return -coef * r.normalized * (smoothRaidus - distance) * (smoothRaidus - distance);
    }

    internal static float ViscosityLaplacian(float magnitude, float coef, float smoothRaidus)
    {
        if (smoothRaidus < magnitude) {
            return 0f;
        }
        return coef * (smoothRaidus - magnitude);
    }
}
