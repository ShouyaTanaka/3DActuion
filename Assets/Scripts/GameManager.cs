using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public Text coinText;
    public GameObject clearPanel;

    [Header("コイン設定")]
    public int totalCoins = 100;
    private int collectedCoins = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateCoinUI();
        if (clearPanel != null)
            clearPanel.SetActive(false);
    }

    public void CollectCoin()
    {
        collectedCoins++;
        UpdateCoinUI();

        Debug.Log($"Collected Coin: {collectedCoins}/{totalCoins}");

        if (collectedCoins >= totalCoins)
        {
            GameClear();
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"Coins: {collectedCoins} / {totalCoins}";
    }

    void GameClear()
    {
        Debug.Log("Game Clear!");
        if (clearPanel != null)
            clearPanel.SetActive(true);
        Time.timeScale = 0f; // 一旦停止（任意）
    }
}
