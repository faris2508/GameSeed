using UnityEngine;

public class TriggerObstacle : MonoBehaviour
{
    public Animator obstacleAnimator;

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            obstacleAnimator.SetTrigger("move");
            hasActivated = true;

            Destroy(gameObject); 
        }
    }
}
