using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int ChunkCoord { get; private set; }

    private List<GameObject> cubes = new List<GameObject>();
    private List<GameObject> coins = new List<GameObject>();

    private float seedX, seedZ;
    private float maxHeight;
    private float relief;
    private bool isSmooth;
    private bool isPerlin;
    private GameObject coinPrefab;
    private int coinCountPerChunk;

    public void Initialize(Vector2Int coord, float seedX, float seedZ,float maxHeight, float relief,bool isSmooth,bool isPerlin, GameObject coinPrefab, int coinCount)
    {
        this.ChunkCoord = coord;
        this.seedX = seedX;
        this.seedZ = seedZ;
        this.maxHeight = maxHeight;
        this.relief = relief;
        this.isSmooth = isSmooth;
        this.isPerlin = isPerlin;
        this.coinPrefab = coinPrefab;
        this.coinCountPerChunk = coinCount;

        GenerateChunk();
    }

    public Bounds ChunkBounds
    {
        get
        {
            Vector3 center = transform.position + new Vector3(ChunkManager.Instance.ChunkSize, 0, ChunkManager.Instance.ChunkSize) * 0.5f;
            Vector3 size = new Vector3(ChunkManager.Instance.ChunkSize, maxHeight + 10f, ChunkManager.Instance.ChunkSize); // 適当に高さ盛ってる
            return new Bounds(center, size);
        }
    }

    void GenerateChunk()
    {
        int chunkSize = ChunkManager.Instance.ChunkSize;
        int offsetX = ChunkCoord.x * chunkSize;
        int offsetZ = ChunkCoord.y * chunkSize;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float y = GetTopY(x + offsetX, z + offsetZ);

                // for (int y = 0; y <= topY; y++)
                // {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = this.transform;
                    cube.transform.localPosition = new Vector3(x, y, z);
                    cube.layer = 6;
                    cubes.Add(cube);
                // }
            }
        }

        // コイン生成
        for (int i = 0; i < coinCountPerChunk; i++)
        {
            int x = Random.Range(0, chunkSize);
            int z = Random.Range(0, chunkSize);
            float y = GetTopY(x + offsetX, z + offsetZ);
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f); // Z軸 or X軸方向に立てる向き
            GameObject coin = Instantiate(coinPrefab, new Vector3(x + offsetX, y + 1f, z + offsetZ), rotation, this.transform);
            coins.Add(coin);
        }
            CombineMeshesForCollider();
    }

    private void CombineMeshesForCollider()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == this.gameObject) continue; // 親自身はスキップ

            // ここでコインを除外する
            if (mf.GetComponent<Coin>() != null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix * transform.worldToLocalMatrix;
            combine.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;

        foreach (GameObject cube in cubes)
        {
            Destroy(cube.GetComponent<Collider>());
        }
    }



    float GetTopY(int x, int z)
    {
        float y = 0;
        if (isPerlin)
        {
            float sampleX = (x + seedX) / relief;
            float sampleZ = (z + seedZ) / relief;
            y = maxHeight * Mathf.PerlinNoise(sampleX, sampleZ);
        }
        else
        {
            y = Random.Range(0, maxHeight);
        }

        return isSmooth ? y : Mathf.Round(y);
    }
}