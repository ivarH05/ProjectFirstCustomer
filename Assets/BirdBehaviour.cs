using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BirdBehaviour : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    bool flying = true;
    float timer = 1;
    float height = 2;
    bool grounded;
    Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 subtracted = transform.position - Player.Position;
        if (subtracted.magnitude < 2)
        {
            rb.useGravity = false;
            subtracted.y = 0;
            rb.AddForce((subtracted.normalized * 750 + new Vector3(Random.Range(-100, 100), Random.Range(50, 100), Random.Range(-100, 100)).normalized * 350) * Time.deltaTime);
            timer = Mathf.Pow(Random.Range(0.8f, 1.5f), 2);
            flying = true;
        }
        else if (Vector3.Distance(transform.position, targetPosition) > 25)
        {
            rb.useGravity = false;
            Vector3 targetDirection = targetPosition - transform.position;
            targetDirection.y *= (height - Random.Range(-1f, 3f));
            rb.velocity = (targetDirection.normalized * 5);
            timer = Mathf.Pow(Random.Range(0.4f, 0.75f), 2);
        }
        if (timer < 0)
        {
            if(rb.velocity.magnitude < 1)
            {
                rb.useGravity = false;
                Vector3 targetDirection = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
                targetDirection.y *= (height - Random.Range(-1f, 3f));
                rb.AddForce(targetDirection.normalized * 500);
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            height = hit.distance;

            if (height < 0.25f)
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

        }

        anim.SetInteger("EventTrigger", Random.Range(0, 100));
        anim.SetBool("Flying", flying);

        timer -= Time.deltaTime;
    }
}
