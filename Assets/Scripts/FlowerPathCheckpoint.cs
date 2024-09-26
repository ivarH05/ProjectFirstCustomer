using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowerPathCheckpoint : MonoBehaviour
{
    public float triggerRadius = 5;
    public float pathWidthMultiplier = 2;
    [HideInInspector]public Vector3 offset;
    [HideInInspector] public Vector3 rotation;
    [HideInInspector] public Vector3 scale;
    Matrix4x4[] matricies = new Matrix4x4[0];
    [HideInInspector] public Terrain terrain;

    private bool animate;
    private int startIndex;
    private int currentIndex;
    private bool done = false;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    private void FixedUpdate()
    {
        if (done) return;

        if(Vector3.Distance(transform.position, Player.Position) < triggerRadius )
            animate = true;

        if (!animate)
            return;

        for(int i = startIndex; i < currentIndex && i < matricies.Length; i++)
        {
            Matrix4x4 matrix = matricies[i]; 
            Vector3 position = matrix.GetColumn(3);

            Vector3 oldscale = new Vector3(
                matrix.GetColumn(0).magnitude,
                matrix.GetColumn(1).magnitude,
                matrix.GetColumn(2).magnitude
            );

            Quaternion rotation = Quaternion.LookRotation(
                matrix.GetColumn(2) / oldscale.z,
                matrix.GetColumn(1) / oldscale.y
            );
            float t = oldscale.y / scale.y;
            matricies[i] = Matrix4x4.TRS(position, rotation, Vector3.Lerp(oldscale, scale, t * Time.deltaTime * 5));
            if (startIndex == i && t > 0.99f)
                startIndex++;
        }

        if (currentIndex < matricies.Length)
            currentIndex += 3;
        if (startIndex >= matricies.Length)
            done = true;
    }

    public void RecalculatePath(float positionVariation, float variation)
    {
        List<Matrix4x4> matrixs = new List<Matrix4x4>();
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();


        for (int i = 2; i < transforms.Length; i++)
        {
            Vector3 previousPoint = transforms[i - 1].position;
            Vector3 nextpoint = transforms[i].position;
            PlantBetweenPoints(matrixs, previousPoint, nextpoint, positionVariation, variation);
        }
        for (int i = 1; i < transforms.Length; i++)
        {
            Destroy(transforms[i].gameObject);
        }
        matricies = matrixs.ToArray();
    }

    void PlantBetweenPoints(List<Matrix4x4> mats, Vector3 start, Vector3 end, float positionVariation, float variation)
    {
        Vector3 right = Vector3.Cross(start - end, Vector3.up).normalized * pathWidthMultiplier;

        float step = 0.15f / Vector3.Distance(start, end);
        if (step < 0.001f)
            step = 0.001f;
        for(float time = 0; time < 1; time += step)
        {
            Vector3 pos = Vector3.Lerp(start, end, time) + RandomXZVector(-positionVariation, positionVariation);
            if (Random.Range(0, 100) < 50)
                pos += right;
            else
                pos -= right;

            pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;
            pos += offset;
            Vector3 euler = RandomXZVector(-variation, variation) * 360 + rotation;
            euler.y = Random.Range(-360, 360);
            Vector3 sca = Vector3.one * 0.01f;
            sca = new Vector3(sca.x * scale.x, sca.y * scale.y, sca.z * scale.z);
            mats.Add(Matrix4x4.TRS(pos, Quaternion.Euler(euler), sca));
        }
    }

    private Vector3 RandomXZVector(float min, float max)
    {
        float multiplier = Random.Range(min, max);
        Vector3 vector = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        return vector.normalized * multiplier;
    }

    public void Draw(Mesh mesh, Material mat)
    {
        Debug.Log("Drawn " + matricies.Length + " instances");
        Graphics.DrawMeshInstanced(mesh, 0, mat, matricies);
    }
}
