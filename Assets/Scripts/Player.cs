using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public float speed = 10f;
    public float jumpPower = 5f;

    private Rigidbody rd;
    private bool canMove = true;

    [Header("Camera")]
    public Transform PlayerCamera;
    public float Sensitivity = 100f;

    void Start()
    {
        rd = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        canMove = true;
    }

    void Update()
    {
        // マウスの動きを取得
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

        // プレイヤーの左右回転（Y軸）
        PlayerCamera.Rotate(Vector3.up * mouseX);


        if(canMove)
        {

        }
    }
}