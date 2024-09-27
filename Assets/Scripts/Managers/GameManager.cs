using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager Singleton;
    public AnimationCurve dropOffFactor;
    public static bool isNight;
    private static bool lastNightCheck;

    public GameObject[] EnableAtNight;
    public GameObject[] DisableAtNight;

    public void Start()
    {
        Singleton = this;
        CameraController.Initialize(Player.camera.transform, dropOffFactor);
        AudioManager.Initialize();
    }
    private void Update()
    {
        CameraController.update();
        AnimalManager.Update();

        if(lastNightCheck == false && isNight)
        {
            for (int i = 0; i < EnableAtNight.Length; i++)
            {
                EnableAtNight[i].SetActive(true);
            }
            for (int i = 0; i < DisableAtNight.Length; i++)
            {
                DisableAtNight[i].SetActive(false);
            }
            lastNightCheck = true;
        }
    }
}
