using UnityEngine;

public class TriggerResetCondition : MonoBehaviour
{
    [Header("GameObject yang akan diaktifkan")]
    public GameObject[] objectsToActivate;

    [Header("GameObject yang akan dinonaktifkan")]
    public GameObject[] objectsToDeactivate;

    [Header("Tag player yang memicu trigger")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Aktifkan semua objek
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            // Nonaktifkan semua objek
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}
