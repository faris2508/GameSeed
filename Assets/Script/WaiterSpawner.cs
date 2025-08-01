using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaiterSpawner : MonoBehaviour
{
    [Header("Waiter Settings")]
    public GameObject waiterPrefab;

    [Header("Waypoint Jalur")]
    public Transform[] leftLaneWaypoints;
    public Transform[] centerLaneWaypoints;
    public Transform[] rightLaneWaypoints;

    [Header("UI Warning")]
    public UIWarningController uiWarning;

    [Header("Movement Settings")]
    public float waiterSpeed = 10f;
    public float destroyDelay = 15f; // Safety timer

    public void SpawnRandomWaiter()
    {
        if (waiterPrefab == null)
        {
            Debug.LogWarning("Waiter prefab belum diassign!");
            return;
        }

        // Pilih jalur random
        int lane = Random.Range(0, 3);

        // Tampilkan tanda seru
        if (uiWarning != null)
            uiWarning.ShowWarning(lane);

        // Tentukan waypoint list sesuai jalur
        Transform[] waypoints = null;
        if (lane == 0) waypoints = leftLaneWaypoints;
        else if (lane == 1) waypoints = centerLaneWaypoints;
        else if (lane == 2) waypoints = rightLaneWaypoints;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("Waypoint untuk lane " + lane + " belum diisi!");
            return;
        }

        // Spawn waiter di posisi waypoint pertama
        GameObject newWaiter = Instantiate(waiterPrefab, waypoints[0].position, waypoints[0].rotation);

        // Mulai coroutine untuk melewati semua waypoint
        StartCoroutine(MoveThroughWaypoints(newWaiter, waypoints));
    }

    private IEnumerator MoveThroughWaypoints(GameObject waiter, Transform[] waypoints)
    {
        int currentWaypoint = 0;
        float timer = 0f;

        while (waiter != null && currentWaypoint < waypoints.Length)
        {
            Vector3 targetPos = waypoints[currentWaypoint].position;

            // Gerakkan menuju waypoint
            while (Vector3.Distance(waiter.transform.position, targetPos) > 0.1f)
            {
                if (waiter == null) yield break;

                waiter.transform.position = Vector3.MoveTowards(
                    waiter.transform.position,
                    targetPos,
                    waiterSpeed * Time.deltaTime
                );

                yield return null;

                timer += Time.deltaTime;
                if (timer > destroyDelay) break;
            }

            // Lanjut ke waypoint berikutnya
            currentWaypoint++;
        }

        if (waiter != null) Destroy(waiter);
    }
}
