using UnityEngine;

public class TrackFollowCamera: MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints; // Titik-titik jalur kamera
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;
    public float flipThreshold = 90f;

    public Vector3 forwardRotation = new Vector3(10, 0, 0);
    public Vector3 backwardRotation = new Vector3(10, 180, 0);

    private int currentWaypoint = 0;
    private bool facingForward = true;

    void LateUpdate()
    {
        if (!player || waypoints.Length == 0) return;

        // 1️⃣ Cari waypoint terdekat
        float shortestDistance = Mathf.Infinity;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(player.position, waypoints[i].position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                currentWaypoint = i;
            }
        }

        // 2️⃣ Dapatkan arah jalur dari waypoint ini
        Transform targetWaypoint = waypoints[currentWaypoint];

        // 3️⃣ Cek arah player untuk flip kamera
        float playerY = Mathf.DeltaAngle(targetWaypoint.eulerAngles.y, player.eulerAngles.y);
        if (playerY > flipThreshold || playerY < -flipThreshold)
            facingForward = false;
        else
            facingForward = true;

        // 4️⃣ Hitung posisi target kamera
        Vector3 desiredPos = targetWaypoint.position +
            (facingForward ? targetWaypoint.rotation * offset : targetWaypoint.rotation * new Vector3(offset.x, offset.y, -offset.z));

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

        // 5️⃣ Hitung rotasi kamera mengikuti arah jalur
        Quaternion targetRotation = Quaternion.Euler(facingForward ? forwardRotation : backwardRotation);
        targetRotation *= Quaternion.Euler(0, targetWaypoint.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
