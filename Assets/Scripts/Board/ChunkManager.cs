using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("チャンクの設定")]
    public int ChunkSize = 20;
    public int ViewDistance = 3;

    [Header("生成用パラメータ")]
    public float seedX = 100f;
    public float seedZ = 100f;
    public float maxHeight = 4f;
    public float relief = 10f;
    public bool isSmooth = true;
    public bool isPerlin = true;
    public GameObject coinPrefab;
    public int coinsPerChunk = 3;
    public bool needCollider = false;
    public int FIRST_GENERATE_CHUNK = 5;

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private Transform player;
    private Vector2Int currentPlayerChunk;

    void Awake()
    {
        Debug.Log("ロード開始");
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastCameraForward = Camera.main.transform.forward;
        // たとえば固定サイズ 10x10 チャンクのマップを前もってすべて生成
        for (int x = -FIRST_GENERATE_CHUNK; x <= FIRST_GENERATE_CHUNK; x++)
        {
            for (int z = -FIRST_GENERATE_CHUNK; z <= FIRST_GENERATE_CHUNK; z++)
            {
                Vector2Int coord = new Vector2Int(x, z);
                if (!chunks.ContainsKey(coord))
                {
                    CreateChunk(coord);
                    chunks[coord].gameObject.SetActive(false); // 非表示にしておく
                }
            }
        }

        currentPlayerChunk = GetPlayerChunkCoord();
        UpdateChunks(); // 初期表示設定
        Debug.Log("ロード完了");
    }

    private Vector3 lastCameraForward;
    void Update()
    {
        Vector2Int newChunk = GetPlayerChunkCoord();
        Vector3 currentCameraForward = Camera.main.transform.forward;

        bool chunkChanged = newChunk != currentPlayerChunk;
        bool cameraTurned = Vector3.Angle(currentCameraForward, lastCameraForward) > 5f; // 角度しきい値

        if (chunkChanged || cameraTurned)
        {
            currentPlayerChunk = newChunk;
            lastCameraForward = currentCameraForward;
            UpdateChunks();
        }
    }


    Vector2Int GetPlayerChunkCoord()
    {
        int x = Mathf.FloorToInt(player.position.x / ChunkSize);
        int z = Mathf.FloorToInt(player.position.z / ChunkSize);
        return new Vector2Int(x, z);
    }

    void UpdateChunks()
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        HashSet<Vector2Int> needed = new HashSet<Vector2Int>();

        // カメラに映っているチャンクを収集
        foreach (var kvp in chunks)
        {
            Bounds bounds = kvp.Value.ChunkBounds;
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
            {
                needed.Add(kvp.Key);
            }
        }

        // プレイヤー周囲も追加
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                Vector2Int coord = currentPlayerChunk + new Vector2Int(dx, dz);
                needed.Add(coord);
            }
        }

        // 必要なチャンクを有効化、不要なものは無効化
        foreach (var kvp in chunks)
        {
            bool shouldBeActive = needed.Contains(kvp.Key);
            kvp.Value.gameObject.SetActive(shouldBeActive);
        }
    }


    void CreateChunk(Vector2Int coord)
    {
        GameObject obj = new GameObject($"Chunk_{coord.x}_{coord.y}");
        obj.transform.position = new Vector3(coord.x * ChunkSize, 0, coord.y * ChunkSize);
        var chunk = obj.AddComponent<Chunk>();

        chunk.Initialize(coord, seedX, seedZ, maxHeight, relief, isSmooth, isPerlin, coinPrefab, coinsPerChunk);
        chunks.Add(coord, chunk);
    }
}