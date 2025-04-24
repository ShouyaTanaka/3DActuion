using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //シード
    private float seedX;
    private float seedZ;
    [SerializeField]
    private float width = 50;
    [SerializeField]
    private float depth = 50;

    //コライダーが必要か
    [SerializeField]
    private bool needToCollider = false;
    [SerializeField]
    private float maxHeight = 10;

    //パーリンノイズを使ったマップか
    [SerializeField]
    private bool isPerlinNoiseMap = true;

    //起伏の激しさ
    [SerializeField]
    private float relief = 15f;

    //Y座標を滑らかにするか(小数点以下をそのままにする)
    [SerializeField]
    private bool isSmoothness = false;

    //マップの大きさ
    [SerializeField]
    private float mapSize = 1f;

    [SerializeField]
    [Header("ーーーーあとから追加ーーーー")]
    private GameObject coinPrefab; // ★ プレハブをInspectorで設定

    [SerializeField]
    private int coinCount = 10; // ★ 生成するコイン数

    private List<GameObject> cubes = new List<GameObject>(); // ★ 地面キューブ記録用
    private List<GameObject> coins = new List<GameObject>();// ★ コイン記録用



    private void Awake ()
    {
        //マップサイズ設定
        transform.localScale = new Vector3(mapSize, mapSize, mapSize);

        //同じマップにならないようにシード生成
        seedX = Random.value * 100f;
        seedZ = Random.value * 100f;

            //キューブ生成
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float topY = GetTopY(x, z); // 一番上の高さを決定

                for (int y = 0; y <= topY; y++) // Y方向に敷き詰め
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = 3;

                    //摩擦０に設定
                    PhysicMaterial noFriction = new PhysicMaterial();
                    noFriction.dynamicFriction = 0f;
                    noFriction.staticFriction = 0f;
                    noFriction.frictionCombine = PhysicMaterialCombine.Minimum;
                    cube.GetComponent<Collider>().material = noFriction;

                    cube.transform.localPosition = new Vector3(x, y, z);
                    cube.transform.SetParent(transform);

                    if (!needToCollider)
                    {
                        Destroy(cube.GetComponent<BoxCollider>());
                    }

                    // 一番上のブロックだけ色付け対象
                    if (y == Mathf.RoundToInt(topY))
                    {
                        SetColor(cube, topY);
                    }

                    cubes.Add(cube);
                }
            }
        }

        SpawnCoins(); // ★ コインを配置

        if (needToCollider)
        {
            CombineMeshesForCollider();
        }
    }

    // ★ ランダムにコインを配置する関数
    private void SpawnCoins()
    {
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
        coins.Clear();

        HashSet<Vector2Int> placedPositions = new HashSet<Vector2Int>();
        int placedCount = 0;

        while (placedCount < coinCount)
        {
            int x = Random.Range(0, (int)width);
            int z = Random.Range(0, (int)depth);
            Vector2Int pos = new Vector2Int(x, z);

            if (placedPositions.Contains(pos)) continue;

            float topY = GetTopY(x, z); // 地形の頂上を取得

            GameObject coin = Instantiate(coinPrefab);
            coin.transform.position = new Vector3(x, topY + 1f, z); // 上に乗せる

            coins.Add(coin);
            placedPositions.Add(pos);
            placedCount++;
        }
    }

    //キューブのY座標を設定する
    private float GetTopY(int x, int z)
    {
        float y = 0;

        if (isPerlinNoiseMap)
        {
            float xSample = (x + seedX) / relief;
            float zSample = (z + seedZ) / relief;
            float noise = Mathf.PerlinNoise(xSample, zSample);
            y = maxHeight * noise;
        }
        else
        {
            y = Random.Range(0, maxHeight);
        }

        if (!isSmoothness)
        {
            y = Mathf.Round(y);
        }

        return y;
    }
    private void SetColor(GameObject cube, float y)
    {
        Color color = Color.black;

        if (y > maxHeight * 0.3f)
        {
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if (y > maxHeight * 0.2f)
        {
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if (y > maxHeight * 0.1f)
        {
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        cube.GetComponent<MeshRenderer>().material.color = color;
    }



    private void CombineMeshesForCollider()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == this.gameObject) continue; // 親自身はスキップ

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 大きなメッシュ用
        combinedMesh.CombineMeshes(combine.ToArray());

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;

        // 個別のコライダーを削除（任意）
        foreach (GameObject cube in cubes)
        {
            Destroy(cube.GetComponent<Collider>());
        }
    }
}
