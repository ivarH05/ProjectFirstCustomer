using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Player
{
    public static PlayerController controller;
    public static InteractSCR interact;

    public static Camera camera { get { return controller.cam; } }
    public static Vector3 Position { get { return controller.PlayerTransform.position; } }
    public static Vector3 FootPosition
    {
        get
        {
            Vector3 pos = controller.PlayerTransform.position;
            float yoffset = controller.cc.height / 2 - controller.cc.center.y;
            return new Vector3(pos.x, pos.y - yoffset, pos.z);
        }
    }

    public static bool HasItem(int id) { return controller.interactscr.HasItem(id); }
    public static bool UseItem(int id) { return controller.interactscr.UseItem(id); }

    public static void PickupItem(int id, GameObject go) { controller.interactscr.PickupItem(go, id); }
    public static void GiveItem(int id) { controller.interactscr.GiveItem(id); }

    public static bool canMove = true;

    public static ProceduralAnimator proceduralAnimator { get { return controller.PlayerMesh.GetComponent<ProceduralAnimator>(); } }
}

public class PlayerController : MonoBehaviour
{
    [Header("Setup")]
    public Transform CameraPan;
    public Transform CameraTilt;

    public Transform PlayerTransform;
    public CharacterController cc;
    public Transform PlayerMesh;
    public Camera cam;
    public AudioSource audioSource;
    public GameObject FootstepDecal;
    public PhysicMaterial[] materials;

    [Header("Settings")]
    public float crouchSpeed = 1.5f;
    public float sneakSpeed = 2.25f;
    public float walkSpeed = 4;
    public float sprintSpeed = 8;
    [Space()]
    public float airControll = 0.075f;
    public float jumpPower = 5;

    public float timeStep = 0.01f;

    [Header("info")]
    public bool isGrounded;
    public bool isSliding;
    public bool isCrouching;
    public bool isSneakning;
    public bool isSprinting;

    public Vector3 velocity;

    private float timer = 0;

    private float shiftTimer;
    private float controllTimer;

    private float lastSinTime;
    private bool AudioFlipSide = true;

    private GameObject[] footsteps = new GameObject[256];
    private GroundData groundData = new GroundData();
    private int footstepIndex;

    [HideInInspector] public InteractSCR interactscr;

    public class GroundData
    {
        public GameObject Ground;
        public Collider Collider;

        public PhysicMaterial physicsMaterial;
        public Vector3 normal;
        public float angle;
        public float distance = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        interactscr = GetComponent<InteractSCR>();
        Player.controller = this;
        Cursor.lockState = CursorLockMode.Locked;
        for (int i = 0; i < footsteps.Length; i++)
        {
            footsteps[i] = Instantiate(FootstepDecal, Vector3.down, Quaternion.Euler(0, 0, 0), transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        while (timer > timeStep)
        {
            timer -= timeStep;
            MoveCharacter();

            velocity.y -= 9.81f * timeStep;
            cc.Move(velocity * timeStep);
        }
        MoveCamera();
        ManageInputs();
        ManageFootsteps();
    }

    private void ManageFootsteps()
    {
        float sin = CameraController.GetSinusTime();
        float speedMultiplier = GetSpeedMultipier();
        if (speedMultiplier < crouchSpeed)
            return;
        if ((AudioFlipSide && sin < lastSinTime) || (!AudioFlipSide && sin > lastSinTime))
        {
            float normalizedSpeed = speedMultiplier / sprintSpeed;
            AudioFlipSide = !AudioFlipSide;
            AudioManager.PlayOneShot(GetFootstepString(), audioSource, Mathf.Pow(normalizedSpeed, 0.25f) * 0.35f, normalizedSpeed * 2);

            RaycastHit hit;
            if (Physics.Raycast(PlayerTransform.position + PlayerTransform.right * (AudioFlipSide ? -0.25f : 0.25f), Vector3.down, out hit, cc.height / 2 + 0.5f))
            {
                Transform f = footsteps[footstepIndex].transform;
                f.position = hit.point;
                f.rotation = Quaternion.LookRotation(hit.normal);
                f.eulerAngles = new Vector3(f.eulerAngles.x, cam.transform.eulerAngles.y, f.eulerAngles.z);
                footstepIndex++;
                if (footstepIndex >= footsteps.Length)
                    footstepIndex = 0;
            }
        }
        lastSinTime = sin;
    }

    private string GetFootstepString()
    {
        PhysicMaterial mat = groundData.physicsMaterial;
        int index = -1;
        for (int i = 0; i < materials.Length; i++)
        {
            PhysicMaterial pm = materials[i];
            if (mat.bounciness == pm.bounciness && mat.dynamicFriction == pm.dynamicFriction && mat.staticFriction == pm.staticFriction)
            {
                index = i;
                break;
            }
        }
        if (index < 0)
            return "PlayerFootstepGrass";

        switch (index)
        {
            case 0:
                return "PlayerFootstepDirt";
            case 1:
                return "PlayerFootstepGrass";
            case 2:
                return "PlayerFootstepGravel";
            case 3:
                return "PlayerFootstepIce";
            case 4:
                return "PlayerFootstepRock";
            case 5:
                return "PlayerFootstepWood";
            default:
                return "PlayerFootstepGrass";
        }
    }

    private void ManageInputs()
    {
        shiftTimer += Time.unscaledDeltaTime;
        if (Input.GetKeyDown(KeyMapping.Sprint))
            shiftTimer = 0;

        if (Input.GetKey(KeyMapping.Sprint) && shiftTimer > UserSettings.inputBuffer)
            isSprinting = true;

        if (Input.GetKeyUp(KeyMapping.Sprint))
        {
            if (shiftTimer > UserSettings.inputBuffer)
            {
                isSneakning = false;
                isSprinting = false;
            }
            else
                isSneakning = !isSneakning;
        }

        controllTimer += Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyMapping.Crouch))
            controllTimer = 0;

        if (Input.GetKey(KeyMapping.Crouch) && (controllTimer > UserSettings.inputBuffer || !isSneakning))
            isCrouching = true;

        if (Input.GetKeyUp(KeyMapping.Crouch))
        {
            if (controllTimer > UserSettings.inputBuffer)
            {
                isCrouching = false;
            }
            else
            {
                isSneakning = false;
                isCrouching = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            CameraController.Explode(2.5f, 1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CameraController.Explode(1.5f, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            CameraController.Explode(1f, 1);
    }

    private void MoveCamera()
    {
        if (CameraController.canMove)
        {
            Vector3 camInput = GetCameraInput();
            CameraPan.localEulerAngles += new Vector3(0, camInput.y, 0);
            float value = camInput.x + CameraTilt.localEulerAngles.x;
            if (value > 180)
                CameraTilt.localRotation = Quaternion.Euler(new Vector3(Mathf.Clamp(value, 280, 720), 0, 0));
            else
                CameraTilt.localRotation = Quaternion.Euler(new Vector3(Mathf.Clamp(value, -360, 80), 0, 0));

            float oldheight = cc.height;
            float newheight = cc.height;
            if (isCrouching)
                newheight = Mathf.Lerp(cc.height, 1.2f, timeStep * 10);
            else
                newheight = Mathf.Lerp(cc.height, 1.8f, timeStep * 10);
            cc.Move(new Vector3(0, newheight - oldheight, 0));
            cc.height = newheight;
            cc.center = new Vector3(0, 0.6f - cc.height / 3, 0);
        }
        else
        {
            CameraTilt.localRotation = Quaternion.Lerp(CameraTilt.localRotation, Quaternion.Euler(CameraController.targetXRotation, 0, 0), Time.deltaTime * 10);

            float oldheight = cc.height;
            float newheight = Mathf.Lerp(cc.height, 1.8f, timeStep * 10);
            cc.Move(new Vector3(0, newheight - oldheight, 0));
            cc.height = newheight;
            cc.center = new Vector3(0, 0.6f - cc.height / 3, 0);
        }
    }

    private void MoveCharacter()
    {

        GroundData ground = GetGroundData();
        isGrounded = ground != null && ground.distance < cc.height / 2 + 0.1f || cc.isGrounded;
        if (ground != null)
        {
            float slidethreshold = 0.5f;
            if (ground.physicsMaterial != null)
                slidethreshold = ground.physicsMaterial.staticFriction;
            if (ground.angle < slidethreshold)
            {
                velocity += ground.normal * timeStep * Mathf.Clamp(3 / ground.distance, 1, 10);
                isSliding = true;
                isGrounded = false;
            }
            else
                isSliding = false;
            groundData = ground;
        }
        else
        {
            isSliding = false;
        }


        Vector2 movementInput = GetMovementInput();
        Vector3 movementVector = CameraPan.forward * movementInput.y + CameraPan.right * movementInput.x;

        float dynamicFriction = 0.5f;
        float staticFriction = 0.5f;

        if (isGrounded)
        {
            velocity.y = -9.81f;
            if (ground != null && ground.physicsMaterial != null)
            {
                dynamicFriction = ground.physicsMaterial.dynamicFriction;
                staticFriction = ground.physicsMaterial.staticFriction;
            }

            if (Input.GetKey(KeyMapping.Jump))
                velocity.y = jumpPower;
        }
        else
        {
            dynamicFriction = airControll / 4;
        }

        velocity = Vector3.Lerp(velocity, new Vector3(0, velocity.y, 0), dynamicFriction);
        float speed = GetSpeedMultipier();
        velocity += movementVector * speed * staticFriction;

        if (isGrounded)
            CameraController.SetHeadBobVariables(speed * 9f, (velocity.magnitude - 7) / 90);
        else
            CameraController.SetHeadBobVariables(0.1f, 0);
    }

    private float GetSpeedMultipier()
    {
        if (GetMovementInput().magnitude == 0)
            return crouchSpeed / 2;
        if (!isGrounded)
            return airControll;
        if (isSliding)
            return airControll;
        if (isCrouching)
            return crouchSpeed;
        if (isSprinting)
            return sprintSpeed;
        if (isSneakning)
            return sneakSpeed;
        return walkSpeed;

    }
    public Vector3 GetCameraInput()
    {
        return new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * UserSettings.MouseSensitivity;
    }

    private Vector2 GetMovementInput()
    {
        if (Player.canMove == false)
            return Vector3.zero;
        Vector2 movement = new Vector2();
        if (Input.GetKey(KeyMapping.MoveForward))
            movement.y++;
        if (Input.GetKey(KeyMapping.MoveBackwards))
            movement.y--;
        if (Input.GetKey(KeyMapping.MoveRight))
            movement.x++;
        if (Input.GetKey(KeyMapping.MoveLeft))
            movement.x--;
        return movement.normalized;
    }

    public GroundData GetGroundData()
    {
        float height = cc.height / 2 + 0.5f;

        float distance = 0.2f;
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, 0, distance),
            new Vector3(0, 0, -distance),
            new Vector3(distance, 0, 0),
            new Vector3(-distance, 0, 0),
            new Vector3(0, 0, 0)
        };
        int hits = 0;
        GroundData result = new GroundData();

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 offset = offsets[i] + cc.center;
            RaycastHit hit;
            if (!Physics.Raycast(PlayerTransform.position + offset, Vector3.down, out hit, height, ~(1 << 7)))
            {
                Debug.DrawRay(PlayerTransform.position + offset, Vector3.down, Color.red);
                continue;
            }
            Debug.DrawLine(PlayerTransform.position + offset, hit.point, Color.green);
            hits++;

            result.Ground = hit.transform.gameObject;
            result.Collider = hit.collider;
            result.normal += hit.normal;
            result.distance += hit.distance;
        }
        if (hits == 0)
            return null;

        result.normal /= hits;
        result.distance /= hits;
        result.angle = Vector3.Dot(result.normal, Vector3.up);
        result.physicsMaterial = result.Collider.material;
        return result;
    }
}
