using UnityEngine;

public class FixedFlipCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public Vector3 forwardRotation = new Vector3(10, 0, 0);
    public Vector3 backwardRotation = new Vector3(10, 180, 0);
    public float flipThreshold = 90f;
    public float smoothSpeed = 5f;

    private bool facingForward = true;

    void LateUpdate()
    {
        if (player == null) return;

        // Cek arah player
        float playerY = Mathf.DeltaAngle(0, player.eulerAngles.y);
        if (playerY > flipThreshold || playerY < -flipThreshold)
            facingForward = false;
        else
            facingForward = true;

        // Tentukan rotasi target
        Quaternion targetRotation = Quaternion.Euler(facingForward ? forwardRotation : backwardRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);

        // POSISI KAMERA: hanya mengikuti gerakan maju/mundur player (Z), X tetap di posisi kamera
        Vector3 camPos = transform.position;

        // Misalnya kita hanya mau kamera mengikuti Z player + offset
        camPos.z = player.position.z + (facingForward ? offset.z : -offset.z);

        // Y kamera tetap bisa ikut offset
        camPos.y = player.position.y + offset.y;

        // X kamera tetap (tidak mengikuti player)
        // Kalau mau ikutin sedikit, bisa pakai Mathf.Lerp di sini

        transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime * smoothSpeed);
    }
}
