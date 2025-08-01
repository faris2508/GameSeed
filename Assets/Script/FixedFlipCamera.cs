using UnityEngine;

public class FixedFlipCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public Vector3 forwardRotation = new Vector3(10, 0, 0);
    public Vector3 backwardRotation = new Vector3(10, 180, 0);
    public float flipThreshold = 90f;
    public float smoothSpeed = 5f;

    private bool facingForward = true;

    void LateUpdate()
    {
        if (player == null) return;

        float playerY = Mathf.DeltaAngle(0, player.eulerAngles.y);

        if (playerY > flipThreshold || playerY < -flipThreshold)
        {
            facingForward = false;
        }
        else
        {
            facingForward = true;
        }

        Quaternion targetRotation = Quaternion.Euler(facingForward ? forwardRotation : backwardRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        transform.position = player.position + targetRotation * offset;
    }
}
