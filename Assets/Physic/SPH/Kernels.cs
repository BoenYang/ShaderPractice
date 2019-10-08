using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Kernels
{
    public static float Poly6(float sqrDistance, float smoothRadius) {
        float coef = 315f / (64f * Mathf.PI * Mathf.Pow(smoothRadius, 9));
        float hSqr = smoothRadius * smoothRadius;
        if (hSqr < sqrDistance) {
            return 0;
        }
        return coef * Mathf.Pow(hSqr - sqrDistance,3);
    }

    public static Vector2 GradientSpiky(Vector2 r, float smoothRaidus)
    {
        float coef = 45f / (Mathf.PI * Mathf.Pow(smoothRaidus, 6));
        float distance = r.magnitude;
        if (smoothRaidus < distance) {
            return Vector2.zero;
        }

        return -coef * r.normalized * Mathf.Pow(smoothRaidus - distance,2);
    }

    internal static float ViscosityLaplacian(float magnitude, float smoothRaidus)
    {
        if (smoothRaidus < magnitude) {
            return 0f;
        }
        float coef = 45f / (Mathf.PI * Mathf.Pow(smoothRaidus, 6));
        return coef * (smoothRaidus - magnitude);
    }
}
