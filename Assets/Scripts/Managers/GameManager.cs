using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AnimationCurve dropOffFactor;
    public void Start()
    {
        print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        CameraController.Initialize(Player.camera.transform, dropOffFactor);
    }
    private void Update()
    {
        CameraController.update();
        AnimalManager.Update();
    }
}
