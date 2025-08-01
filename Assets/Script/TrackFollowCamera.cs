using UnityEngine;

public class TrackFollowCamera : MonoBehaviour
{
    public Transform player;
    public Transform trackDirectionTarget;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null || trackDirectionTarget == null) return;

        Vector3 desiredPosition = player.position + transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(trackDirectionTarget.position - player.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
