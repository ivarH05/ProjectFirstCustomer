using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public enum NPCBehaviour
{
    idle,
    walk,
    eat,
    regroup,
    flee,
    scatter,
    alerted,
    runToPoint
}

public class AnimalBehaviour : MonoBehaviour
{
    [Header("Setup")]
    public Animator behaviourTree;
    public Transform target;
    public NavMeshAgent agent;

    [Header("Settings")]
    public float walkSpeed;
    public float sprintSpeed;
    public float hearing = 1;

    internal NPCBehaviour behaviour;

    internal Vector3 velocity;
    internal float randomEventTimer;

    [Header("info")]
    public float Alertness = 0;
    private float speed = 1;
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        speed = walkSpeed;
        AnimalManager.AddAnimal(this);
    }

    // Update is called once per frame
    void Update()
    {
        randomEventTimer -= Time.deltaTime;
        velocity = (transform.position - lastPos) / Time.deltaTime;
        lastPos = transform.position;
        Alertness = Mathf.Lerp(Alertness, 0, Time.deltaTime / 10);

        UpdateVariables();
    }

    public void UpdateBehaviour()
    {
        behaviour = checkState();
        switch (behaviour)
        {
            case NPCBehaviour.idle:
                IdleBehavior();
                break;
            case NPCBehaviour.walk:
                WalkBehavior();
                break;
            case NPCBehaviour.eat:
                EatBehavior();
                break;
            case NPCBehaviour.scatter:
                ScatterBehavior();
                break;
            case NPCBehaviour.alerted:
                AlertedBehavior();
                break;
            case NPCBehaviour.runToPoint:
                RunToPointBehavior();
                break;
            case NPCBehaviour.flee:
                FleeBehavior();
                break;
            case NPCBehaviour.regroup:
                RegroupBehavior();
                break;
        }
    }
    public void PlaySound(Vector3 position, float volume)
    {
        float distance = Vector3.Distance(transform.position, position) - 2.5f;
        float loudness = (1 / (distance * distance)) * volume;
        if(1 - loudness < hearing)
            behaviourTree.SetTrigger("HeardSound");
        Alertness += loudness;
    }

    virtual internal void IdleBehavior()
    {
        if(randomEventTimer < 0)
        {
            randomEventTimer = Random.Range(0, 10);
            behaviourTree.SetTrigger("RandomEvent");
        }
        lerpSpeed(walkSpeed);
    }

    virtual internal void WalkBehavior()
    {
        if (agent.remainingDistance < 1 || agent.isStopped)
            RandomMovement();
        lerpSpeed(walkSpeed);
    }

    virtual internal void EatBehavior()
    {
        agent.isStopped = true;
        lerpSpeed(walkSpeed / 2);
    }
    virtual internal void ScatterBehavior()
    {
        lerpSpeed(sprintSpeed);
        if (agent.remainingDistance < 1 || agent.isStopped)
        {
            Vector3 subtracted = transform.position - target.position;
            Vector3 normalized = subtracted.normalized;
            Vector3 desitnation = transform.position + normalized * 10 + RandomXZVector(2, 5);
            agent.isStopped = false;
            agent.SetDestination(desitnation);
        }
    }
    virtual internal void AlertedBehavior()
    {
        agent.isStopped = true;
        lerpSpeed(sprintSpeed);
    }
    virtual internal void RunToPointBehavior()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
        lerpSpeed(sprintSpeed);
    }

    virtual internal void FleeBehavior()
    {
        lerpSpeed(sprintSpeed);
        if (agent.remainingDistance < 1 || agent.isStopped)
        {
            Vector3 subtracted = transform.position - Player.Position;
            Vector3 normalized = subtracted.normalized;
            Vector3 desitnation = transform.position + normalized * 7 + RandomXZVector(0, 5);
            agent.isStopped = false;
            agent.SetDestination(desitnation);
        }
    }

    virtual internal void RegroupBehavior()
    {

    }

    public void RandomMovement()
    {
        agent.isStopped = false;
        agent.SetDestination(transform.position + RandomXZVector(5, 10));
    }

    private Vector3 RandomVector(float min, float max)
    {
        float multiplier = Random.Range(min, max);
        Vector3 vector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        return vector.normalized * multiplier;
    }

    private Vector3 RandomXZVector(float min, float max)
    {
        float multiplier = Random.Range(min, max);
        Vector3 vector = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        return vector.normalized * multiplier;
    }

    private void lerpSpeed(float targetSpeed)
    {
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);
    }

    NPCBehaviour checkState()
    {
        AnimatorStateInfo state = behaviourTree.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Alerted"))
            return NPCBehaviour.alerted;
        if (state.IsName("RunToPoint"))
            return NPCBehaviour.runToPoint;
        if (state.IsName("ScatterFromPoint"))
            return NPCBehaviour.scatter;
        if (state.IsName("Flee"))
            return NPCBehaviour.flee;
        if (state.IsName("Idle"))
            return NPCBehaviour.idle;
        if (state.IsName("Regroup"))
            return NPCBehaviour.regroup;
        if (state.IsName("Eat"))
            return NPCBehaviour.eat;
        if (state.IsName("Walk"))
            return NPCBehaviour.walk;
        return NPCBehaviour.idle;
    }

    void UpdateVariables()
    {
        behaviourTree.SetFloat("PlayerDistance", Vector3.Distance(transform.position, Player.Position));
        behaviourTree.SetFloat("Speed", velocity.magnitude);
        behaviourTree.SetFloat("GroupScatterValue", 0);

        behaviourTree.SetFloat("Alertness", Alertness);
    }

    private void OnDestroy()
    {
        AnimalManager.RemoveAnimal(this);
    }
}
