using UnityEngine;

public class ThrownFood : MonoBehaviour
{
    public Inventory inventory; // Referensi ke inventory pemain
    private AudioClip smashSound; // Variabel private untuk menampung klip audio

    private bool hasHitCustomer = false;
     // Metode untuk menerima sound effect
    public void SetSmashSound(AudioClip clip)
    {
        smashSound = clip;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Customer"))
        {
            // Tambahkan poin jika mengenai customer
            if (inventory != null)
            {
                inventory.AddPoints(10); // Misalnya 10 poin per makanan
            }
            hasHitCustomer = true;
            Destroy(gameObject); // Hapus makanan setelah mengenai customer
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Hapus makanan jika mengenai tanah (tanpa poin)
            AudioSource.PlayClipAtPoint(smashSound, transform.position);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Sound effect hanya akan dimainkan jika belum mengenai customer
        // Ini memastikan suara hanya dimainkan saat piring pecah
        if (!hasHitCustomer && smashSound != null)
        {
            AudioSource.PlayClipAtPoint(smashSound, transform.position);
        }
    }
    private void Start()
    {
        // Hapus makanan setelah 5 detik jika tidak mengenai apa pun
        Destroy(gameObject, 1f);

    }
}