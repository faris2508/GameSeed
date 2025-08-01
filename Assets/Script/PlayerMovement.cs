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
    [SerializeField] private Transform model;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.linearDamping = 0.1f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);

        float moveInput = Input.GetAxis("Vertical");

        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        bool isPressingW = moveInput > 0;

        if (Input.GetKey(KeyCode.Space) && isMoving)
        {
            rb.linearDamping = brakeDrag;
            animator.SetBool("isBraking", true);

            animator.SetBool("isWalking", false);
            animator.SetBool("isIdleWalk", false);
            animator.SetBool("isIdle", false);
        }
        else
        {
            rb.linearDamping = 0.1f;
            animator.SetBool("isBraking", false);

            if (!isMoving)
            {
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdleWalk", false);
            }
            else if (isPressingW)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdleWalk", false);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isIdleWalk", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdle", false);
            }
        }

        if (isPressingW)
        {
            Vector3 moveForce = transform.forward * moveInput * moveSpeed;
            rb.AddForce(moveForce, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else
        {
            rb.linearVelocity *= (1 - deceleration * Time.fixedDeltaTime);
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

        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        animator.SetTrigger("Idle");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        isDead = false;
    }
}
