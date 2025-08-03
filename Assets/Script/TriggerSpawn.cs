using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    [Header("Spawner Settings")]
    public WaiterSpawner waiterSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (waiterSpawner != null)
            {
                waiterSpawner.SpawnRandomWaiter();
            }

            // Matikan trigger setelah digunakan
            gameObject.SetActive(false);
        }
    }
}