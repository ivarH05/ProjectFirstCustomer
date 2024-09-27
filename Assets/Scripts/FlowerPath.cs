using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPath : MonoBehaviour
{
    public Mesh mesh;
    public Material StandardFlowerMaterial;
    public Terrain terrain;
    public Vector3 offset;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;


    FlowerPathCheckpoint[] checkpoints;
    public float positionVariation;
    public float variation;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = GetComponentsInChildren<FlowerPathCheckpoint>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            FlowerPathCheckpoint c = checkpoints[i];
            c.offset = offset;
            c.rotation = rotation;
            c.scale = scale;
            c.terrain = terrain;
            c.RecalculatePath(positionVariation, variation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].Draw(mesh, StandardFlowerMaterial);
        }
    }
}
