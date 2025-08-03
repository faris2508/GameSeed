using UnityEngine;

public class CustomerIdentity : MonoBehaviour
{
    public CustomerSO customerData;
    public Transform orderSpawnPoint; // drag di prefab
    public GameObject currentOrderUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelUpdate.Instance.OnCustomerTriggerEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelUpdate.Instance.OnCustomerTriggerExit(this);
        }
    }
}
