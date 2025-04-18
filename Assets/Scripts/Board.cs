using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //シード
    private float seedX;
    private float seedZ;

    //マップのサイズ
    [SerializeField]
    [Header("------実行中に変えれない------")]
    private float width = 50;
    [SerializeField]
    private float depth = 50;

    //コライダーが必要か
    [SerializeField]
    private bool needToCollider = false;

    [SerializeField]
    [Header("------実行中に変えられる------")]
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
    private GameObject coinPrefab; // ★ プレハブをInspectorで設定

    [SerializeField]
    private int coinCount = 10; // ★ 生成するコイン数

    private List<GameObject> cubes = new List<GameObject>(); // ★ 地面キューブ記録用


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
                //新しいキューブ作成、平面に置く
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.layer = 3;
                cube.transform.localPosition = new Vector3 (x, 0, z);
                cube.transform.SetParent (transform);

                if(!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider> ());
                }
                //高さ設定
                SetY (cube);
                cubes.Add(cube); // ★ 地面を記録
            }
        }

        SpawnCoins(); // ★ コインを配置
    }

    // ★ ランダムにコインを配置する関数
    private void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            int index = Random.Range(0, cubes.Count);
            GameObject targetCube = cubes[index];

            Vector3 coinPos = targetCube.transform.position + Vector3.up * 1f;
            Instantiate(coinPrefab, coinPos, Quaternion.identity);
        }
    }

    //インスペクターの値が変更された時
    private void OnValidate ()
    {
        //実行中でなければスルー
        if(!Application.isPlaying)
        {
            return;
        }

        //マップの大きさ設定
        transform.localScale = new Vector3(mapSize, mapSize, mapSize);

        //各キューブのY座標変更
        foreach (Transform child in transform)
        {
            SetY (child.gameObject);
        }
    }

    //キューブのY座標を設定する
    private void SetY(GameObject cube)
    {
        float y = 0;

        //パーリンノイズを使って高さを決める場合
        if(isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);

            y = maxHeight * noise;
        }
        //完全ランダムで高さを決める場合
        else
        {
            y = Random.Range (0, maxHeight);
        }

        //滑らかに変化しない場合はyを四捨五入
        if(!isSmoothness)
        {
            y = Mathf.Round (y);
        }

        //位置設定
        cube.transform.localPosition = new Vector3 (cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        //高さによって色を段階的に変更
        Color color = Color.black;

        if(y > maxHeight * 0.3f)
        {
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if(y > maxHeight * 0.2f)
        {
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if(y > maxHeight * 0.1f)
        {
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        cube.GetComponent<MeshRenderer> ().material.color = color;
    }
}
