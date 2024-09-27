using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCameraController : MonoBehaviour
{
    Vector3 velocity;
    Vector3 targetRot;
    public float roughness = 1;
    public float FOV = 90;
    public float speed = 1;
    public Camera cam;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            active = !active;

        if (!active)
            return;

        if(Input.mouseScrollDelta.y != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                speed *= Input.mouseScrollDelta.y < 0 ? 0.9f : 1.1f;
            else if (Input.GetKey(KeyCode.LeftControl))
                roughness *= Input.mouseScrollDelta.y < 0 ? 0.9f : 1.1f;
            else
                FOV *= Input.mouseScrollDelta.y > 0 ? 0.9f : 1.1f;
        }

        Vector3 input = GetMovementInput();

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV, Time.deltaTime * roughness);
        velocity += cam.transform.forward * input.z;
        velocity += cam.transform.right * input.x;
        velocity += cam.transform.up * input.y;
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * roughness / 2);

        targetRot += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);

        cam.transform.position += velocity * Time.deltaTime;
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * roughness);

        if (Input.GetKeyDown(KeyCode.Space))
            AnimalManager.PlaySound(transform.position, 1000);
    }
    private Vector3 GetMovementInput()
    {
        if (Player.canMove == false)
            return Vector3.zero;
        Vector3 movement = new Vector3();
        if (Input.GetKey(KeyMapping.MoveForward))
            movement.z++;
        if (Input.GetKey(KeyMapping.MoveBackwards))
            movement.z--;
        if (Input.GetKey(KeyMapping.MoveRight))
            movement.x++;
        if (Input.GetKey(KeyMapping.MoveLeft))
            movement.x--;
        if (Input.GetKey(KeyCode.E))
            movement.y++;
        if (Input.GetKey(KeyCode.Q))
            movement.y--;
        return movement.normalized * speed * Time.deltaTime * roughness;
    }
}
