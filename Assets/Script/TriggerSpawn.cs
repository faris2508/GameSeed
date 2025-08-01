using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    [Header("Spawner Settings")]
    public WaiterSpawner waiterSpawner; 
    public bool oneTimeTrigger = true; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (waiterSpawner != null)
            {
                waiterSpawner.SpawnRandomWaiter();
            }

            if (oneTimeTrigger)
                hasTriggered = true;
        }
    }
}
