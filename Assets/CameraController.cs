using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class CameraController
{
    private static AnimationCurve dropOffFactor;
    private static Transform cam;

    private static float headbobTime;
    private static float headbobSpeed;
    private static float headbobModifier;
    private static float lerpedModifier;

    private static float explosionDuration;
    private static float explosionTime;
    private static float explosionModifier;

    public static void Initialize(Transform camTransform, AnimationCurve dropOffCurve)
    {
        cam = camTransform;
    }
    public static void update()
    {
        headbobTime += Time.deltaTime * headbobSpeed;
        explosionTime += Time.deltaTime;
        lerpedModifier = Mathf.Lerp(lerpedModifier, headbobModifier, Time.deltaTime * 10);
        Debug.Log(lerpedModifier);

        Vector3 vec = new Vector3(Mathf.Sin(headbobTime), Mathf.Sin(headbobTime * 2) / 2, 0) * lerpedModifier;
        cam.localPosition = vec;
        cam.localEulerAngles = new Vector3(vec.y, vec.x, 0) * 30;

        if (explosionTime > explosionDuration)
            return;
        float multiplier = dropOffFactor.Evaluate(explosionTime / explosionDuration) * explosionModifier;
        cam.localPosition += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * multiplier;
    }

    public static void SetHeadBobVariables(float Speed, float Modifier)
    {
        headbobSpeed = Speed;
        headbobModifier = Modifier * Modifier;
    }

    public static void Explode(float Duration, float Strength)
    {
        explosionDuration = Duration;
        explosionTime = 0;
        explosionModifier = Strength;
    }
}
