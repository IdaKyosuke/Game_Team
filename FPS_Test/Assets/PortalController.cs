using UnityEngine;

public class PortalController : MonoBehaviour
{
    float rayDistance = 2.5f;

    //12番目のレイヤー(自身)を除外したLayerMask
    LayerMask excludeLayer12 = ~(1 << 12);

    void Start()
    {
        CheckAndRotate(0);
    }

    void CheckAndRotate(int rotateCount)
    {
        // 最大4回まで回転
        if (rotateCount >= 4) return;

        // 中心より少し前からRayを出す
        Vector3 rayOrigin = transform.position + transform.forward * 0.5f;
        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, rayDistance, excludeLayer12))
        {
            // 壁に当たった場合90度回転（Y軸）
            transform.Rotate(0, 90, 0);

            // もう一度確認（回転回数を増やす）
            CheckAndRotate(rotateCount + 1);
        }
    }
}