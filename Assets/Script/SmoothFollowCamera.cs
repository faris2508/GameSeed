using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float rotationSpeed = 5f;
    public float positionSpeed = 5f;
    public float lookAtSpeed = 5f;
    public float maxRotationAngle = 80f; // Batas rotasi kamera saat belokan tajam

    void LateUpdate()
    {
        if (player == null) return;

        // Hitung posisi target kamera berdasarkan rotasi player
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);
        
        // Smooth movement untuk posisi kamera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSpeed * Time.deltaTime);

        // Hitung rotasi target (mengarah ke player dengan sedikit melihat ke bawah)
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position + Vector3.up * 0.5f, Vector3.up);
        
        // Smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Optional: Batasi rotasi kamera saat belokan tajam
        float currentAngle = Vector3.Angle(transform.forward, player.forward);
        if (currentAngle > maxRotationAngle)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(player.forward, Vector3.up), 
                (currentAngle - maxRotationAngle));
        }
    }
}