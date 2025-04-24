using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MonoBehaviour
{
    public Transform player;     // プレイヤーのTransform
    public RectTransform icon;   // ミニマップ上のアイコン

    void Update()
    {
        if (player == null || icon == null) return;

        // プレイヤーのY軸回転だけ取得して反映
        float yRotation = player.eulerAngles.y;
        icon.localRotation = Quaternion.Euler(0, 0, -yRotation);
    }
}