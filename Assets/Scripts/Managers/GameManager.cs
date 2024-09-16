using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AnimationCurve dropOffFactor;
    private void Start()
    {
        CameraController.Initialize(Player.camera.transform, dropOffFactor);
    }
    private void Update()
    {
        CameraController.update();
    }
}
