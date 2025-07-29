using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public float moveSpeed = 15f;
    public float maxSpeed = 25f;
    public float turnSpeed = 120f;
    public float deceleration = 0.3f;
    public float brakeDrag = 2f;

    public Transform respawnPoint; 
    public float respawnDelay = 2f; 

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.linearDamping = 0.1f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (isDead) return; 

        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);

        float moveInput = Input.GetAxis("Vertical");
        if (moveInput > 0)
        {
            Vector3 moveForce = transform.forward * moveInput * moveSpeed;
            rb.AddForce(moveForce, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else
        {
            rb.linearVelocity = rb.linearVelocity * (1 - deceleration * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearDamping = brakeDrag;
        }
        else
        {
            rb.linearDamping = 0.1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Waiters") && !isDead)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death"); 
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(respawnDelay);
        animator.SetTrigger("Idle");

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        isDead = false;
    }
}
