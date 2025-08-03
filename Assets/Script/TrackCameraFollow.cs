using UnityEngine;

public class TrackCameraFollow : MonoBehaviour
{
    public Transform player;         // Target player

    [Header("Posisi Kamera")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Offset posisi dari player
    public float followSpeed = 5f;   // Kecepatan kamera mengikuti

    [Header("Rotasi Kamera")]
    public Vector3 rotationOffset = Vector3.zero; // Offset rotasi awal kamera

    [Header("Posisi Jalur")]
    public float fixedX = 0f;        // Posisi X tetap di tengah lintasan

    void LateUpdate()
    {
        if (player == null) return;

        // Posisi target kamera (X tetap di jalur, Y & Z mengikuti player + offset)
        Vector3 targetPos = new Vector3(
            fixedX + offset.x,                       // tetap di jalur + offset X
            player.position.y + offset.y,            // tinggi kamera
            player.position.z + offset.z             // jarak di belakang player
        );

        // Smooth mengikuti posisi
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // Rotasi kamera menghadap player
        transform.LookAt(player.position + Vector3.up * 1f);

        // Tambahkan rotasi offset supaya posisi awal kamera bisa disesuaikan
        transform.rotation *= Quaternion.Euler(rotationOffset);
    }
}
