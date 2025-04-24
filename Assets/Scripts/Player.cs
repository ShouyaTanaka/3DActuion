using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FPS風プレイヤー制御（移動・ジャンプ・視点制御）
/// </summary>
public class Player : MonoBehaviour
{
    public float speed = 6f;
    public float mouseSensitivity = 2f;
    public float jumpPower = 4f;
    public Camera playerCamera;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // マウスカーソルを非表示＆固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
        Move();
        Jump();
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // カメラの上下回転（X軸）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // プレイヤー本体の左右回転（Y軸）
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        // カメラの正面基準で移動方向を決定
        Vector3 moveDir = transform.right * h + transform.forward * v;
        moveDir.Normalize();

        // 水平方向の速度はプレイヤーの入力、垂直方向は現在の速度を保持
        Vector3 velocity = moveDir * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // 地面との接触判定（Raycastでチェック）
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.15f);
        Debug.DrawRay(transform.position, Vector3.down * 1.15f, isGrounded ? Color.green : Color.red);
    }
}