using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Singleton;
    public AnimationCurve dropOffFactor;
    public GameObject BirdNest;
    public void Start()
    {
        CameraController.Initialize(Player.camera.transform, dropOffFactor);
        AudioManager.Initialize();
        Singleton = this;
    }
    private void Update()
    {
        CameraController.update();
        AnimalManager.Update();
    }

    public static GameObject getBirdNest()
    {
        return Singleton.BirdNest;
    }
}
