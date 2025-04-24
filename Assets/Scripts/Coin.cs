using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 90f; // 回転速度（度/秒）
    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCoin();
            Destroy(gameObject);
        }
    }
}