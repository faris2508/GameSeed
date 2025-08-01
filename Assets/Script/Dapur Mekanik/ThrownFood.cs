using UnityEngine;

public class ThrownFood : MonoBehaviour
{
    public Inventory inventory; // Referensi ke inventory pemain

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Customer"))
        {
            // Tambahkan poin jika mengenai customer
            if (inventory != null)
            {
                inventory.AddPoints(10); // Misalnya 10 poin per makanan
            }
            Destroy(gameObject); // Hapus makanan setelah mengenai customer
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Hapus makanan jika mengenai tanah (tanpa poin)
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hapus makanan setelah 5 detik jika tidak mengenai apa pun
        Destroy(gameObject, 5f);
    }
}