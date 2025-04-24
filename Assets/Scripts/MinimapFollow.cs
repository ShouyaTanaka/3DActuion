using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;  // プレイヤーのTransform

    void LateUpdate()
    {
        if (target == null) return;

        // ミニマップカメラをプレイヤーの真上に移動
        Vector3 newPos = target.position;
        newPos.y = 50f; // カメラの高さ（適宜調整）
        transform.position = newPos;
    }
}
