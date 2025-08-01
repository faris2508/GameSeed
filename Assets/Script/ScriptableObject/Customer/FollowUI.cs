using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public Transform target; // GameObject 3D yang diikuti
    public Vector3 offset;   // Offset posisi dari atas object
    public Camera mainCamera; // Kamera utama
    public Image pesanan;

    private RectTransform rectTransform;

    void Start()
    {
        
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null || mainCamera == null)
            return;

        // Konversi world position ke screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);

        // Jika target ada di depan kamera
        if (screenPos.z > 0)
        {
            rectTransform.position = screenPos;
        }
        else
        {
            // Target di belakang kamera, bisa sembunyikan
            rectTransform.position = new Vector3(-1000, -1000, 0);
        }
    }
}
