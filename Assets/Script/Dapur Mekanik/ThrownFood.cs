using UnityEngine;

public class ThrownFood : MonoBehaviour
{
    public Inventory inventory;
    private AudioClip smashSound;
    private bool hasHitCustomer = false;
    public string foodName;

    public void SetSmashSound(AudioClip clip)
    {
        smashSound = clip;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Customer"))
        {
            Customer customer = collision.gameObject.GetComponent<Customer>();
            if (customer != null)
            {
                customer.CheckOrder(foodName); // Skrip Customer yang akan menangani
            }
            hasHitCustomer = true;
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (inventory != null)
            {
                inventory.SubtractPoints(2); // Kurangi poin saat mengenai tanah
            }
            AudioSource.PlayClipAtPoint(smashSound, transform.position);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!hasHitCustomer && smashSound != null)
        {
            // Sound effect hanya dimainkan jika pecah di tanah,
            // bukan saat kena pelanggan
        }
    }

    private void Start()
    {
        Destroy(gameObject, 1f);
    }
}