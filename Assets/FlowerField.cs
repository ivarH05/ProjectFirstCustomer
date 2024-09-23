using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerField : MonoBehaviour
{
    [Header("Setup")]
    public Terrain terrain;

    [Header("Settings")]
    public Mesh mesh;
    [Tooltip("Make sure the material has GPU instancing enabled at the bottom")]
    public Material material;
    public Vector3 offset;
    private Vector2Int size = new Vector2Int(30, 30);
    public Vector3 offsetAfter;

    [Header("Transform")]
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = new Vector3(1, 1, 1);
    public float positionRandomization = 5f;
    public float randomization = 0.1f;

    private Matrix4x4[] matricies;
    private Vector3[] positions;
    private Vector3[] rotations;
    private Vector3[] scales;
    private bool[] done;

    // Start is called before the first frame update
    void Start()
    {
        matricies = new Matrix4x4[size.x * size.y];
        positions = new Vector3[size.x * size.y];
        rotations = new Vector3[size.x * size.y];
        scales = new Vector3[size.x * size.y];
        done = new bool[size.x * size.y];

        Vector3 basePos = transform.position + position;
        Vector3 baseRot = transform.eulerAngles + rotation;
        Vector3 baseSca = new Vector3(transform.localScale.x * scale.x, transform.localScale.y * scale.y, transform.localScale.z * scale.z);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int index = y * size.x + x;
                Vector3 pos = basePos + new Vector3(offset.x * (x - size.x / 2), 0, offset.z * (y - size.y / 2)) + RandomXZVector(-positionRandomization, positionRandomization);
                pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;
                Vector3 rot = baseRot + RandomXZVector(-randomization * 360, randomization * 360);
                rot.y = Random.Range(-360, 360);
                Vector3 sca = baseSca + RandomXZVector(-randomization, randomization);
                Quaternion qua = Quaternion.Euler(rot);

                matricies[index] = Matrix4x4.TRS(pos, qua, sca);
                positions[index] = pos;
                rotations[index] = rot;
                scales[index] = sca;
                done[index] = false;
            }
        }
    }
    private Vector3 RandomXZVector(float min, float max)
    {
        float multiplier = Random.Range(min, max);
        Vector3 vector = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        return vector.normalized * multiplier;
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matricies);
        Vector3 playerPos = Player.FootPosition;
        float playerDist = Vector3.Distance(transform.position, playerPos);
        if (playerDist > size.x * offset.x && playerDist > size.y * offset.y)
            return;
        DisplaceScale();
    }
    public void DisplaceScale()
    {
        Vector3 playerPos = Player.FootPosition;
        int index = 0;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3 pos = positions[index];
                float dist = Vector3.Distance(playerPos, pos);
                const float effectDistance = 01f;
                if (dist > effectDistance)
                    goto end;

                float newScale = Mathf.Clamp(dist / effectDistance, 0.01f, 1) * scales[index].y;
                if (newScale > matricies[index].GetS().y)
                    goto end;
                matricies[index].SetTRS
                (
                    pos + offsetAfter,
                    Quaternion.Euler(rotations[index]),
                    new Vector3
                    (
                        scales[index].x,
                        newScale,
                        scales[index].z
                    )
                );
                done[index] = true;

                end:
                index++;
            }
        }
    }
}
