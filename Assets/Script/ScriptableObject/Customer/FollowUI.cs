using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public Transform target; // Posisi yang akan diikuti (biasanya customer.orderSpawnPoint)
    public Vector3 offset = new Vector3(0, 2f, 0); // Offset di atas kepala

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            //transform.LookAt(Camera.main.transform); // Supaya selalu menghadap kamera
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
