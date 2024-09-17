using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AnimationCurve dropOffFactor;
    public void Start()
    {
        CameraController.Initialize(Player.camera.transform, dropOffFactor);
        AudioManager.Initialize();
    }
    private void Update()
    {
        CameraController.update();
        AnimalManager.Update();
    }
}
