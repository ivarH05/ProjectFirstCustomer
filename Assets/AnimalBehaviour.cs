using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Animations;
using UnityEngine;

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
    public Animator behaviourTree;
    NPCBehaviour behaviour;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        AnimalManager.AddAnimal(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVariables();
    }

    public void UpdateBehaviour()
    {
        behaviour = checkState();

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
        behaviourTree.SetFloat("GroupScatter", 0);
        behaviourTree.SetFloat("Slertness", 0);

    }

    private void OnDestroy()
    {
        AnimalManager.RemoveAnimal(this);
    }
}
