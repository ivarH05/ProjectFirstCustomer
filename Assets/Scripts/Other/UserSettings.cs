using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyMapping
{
    public static KeyCode MoveForward = KeyCode.W;
    public static KeyCode MoveBackwards = KeyCode.S;
    public static KeyCode MoveLeft = KeyCode.A;
    public static KeyCode MoveRight = KeyCode.D;

    public static KeyCode Jump = KeyCode.Space;
    public static KeyCode Sprint = KeyCode.LeftShift;
    public static KeyCode Crouch = KeyCode.LeftControl;

    public static KeyCode Interact = KeyCode.E;
}

public static class UserSettings
{
    public static float MouseSensitivity = 2.5f;
    public static float inputBuffer = 0.2f;
}
