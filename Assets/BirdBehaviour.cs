using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    bool flying = true;
    float timer = 1;
    bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 0)
        {
            if(rb.velocity.magnitude < 1)
            {
                rb.useGravity = false;
                rb.AddForce(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)).normalized * 500);
                timer = Mathf.Pow(Random.Range(0.4f, 0.75f), 2);
            }
            else
            {
                rb.velocity = Vector3.zero;
                timer = Random.Range(0.25f, 1.25f);
            }
        }
        if (rb.velocity.magnitude > 1)
        {
            Vector3 direction = rb.velocity;
            direction.y = 0;
            direction = direction.normalized;
            Vector3 euler = Quaternion.LookRotation(direction).eulerAngles;
            euler.y -= 90;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), Time.deltaTime * 15);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.25f))
        {
            if (!grounded)
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                timer = Random.Range(3f, 10f);
                flying = false;
                grounded = true;
            }

        }
        else
        {
            grounded = false;
            flying = true;
        }

        anim.SetInteger("EventTrigger", Random.Range(0, 100));
        anim.SetBool("Flying", flying);

        timer -= Time.deltaTime;
    }
}
