using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public Animator animator;

    public float moveSpeed = 15f;
    public float maxSpeed = 25f;
    public float turnSpeed = 120f;
    public float deceleration = 0.3f;
    public float brakeDrag = 2f;

    public float driftTurnMultiplier = 1.5f;
    public float driftDrag = 0.8f;
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    public ParticleSystem driftParticles;
    public ParticleSystem brakeParticles;

    private bool isDead = false;
    [SerializeField] private Transform model;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip deathSFX;

    public Inventory playerInventory;
    [SerializeField] private bool isCarryingItem;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.linearDamping = 0.1f;
        rb.useGravity = true;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (playerInventory != null)
        {
            isCarryingItem = playerInventory.isCarryingItem;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.angularVelocity = Vector3.zero;

        float turnInput = Input.GetAxis("Horizontal");
        float moveInput = Input.GetAxis("Vertical");
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        bool isPressingW = moveInput > 0;
        bool isCarrying = isCarryingItem;

        ResetAllAnimatorBools();

        // DRIFT
        if (Input.GetKey(KeyCode.LeftShift) && isMoving)
        {
            transform.Rotate(0, turnInput * turnSpeed * driftTurnMultiplier * Time.fixedDeltaTime, 0);
            rb.linearDamping = driftDrag;
            animator.SetBool("isDrifting", true);
            if (driftParticles != null && !driftParticles.isPlaying) driftParticles.Play();
        }
        else
        {
            transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
            if (driftParticles != null && driftParticles.isPlaying) driftParticles.Stop();
        }

        // BRAKING
        if (Input.GetKey(KeyCode.S) && isMoving && !Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearDamping = brakeDrag;
            if (isCarrying)
                animator.SetBool("isCarryingBraking", true);
            else
                animator.SetBool("isBraking", true);

            if (brakeParticles != null && !brakeParticles.isPlaying) brakeParticles.Play();
        }
        else
        {
            rb.linearDamping = 0.1f;
            if (brakeParticles != null && brakeParticles.isPlaying) brakeParticles.Stop();

            // ANIMASI IDLE / WALK / IDLEWALK
            if (!isMoving)
            {
                if (isCarrying)
                    animator.SetBool("isCarryingIdle", true);
                else
                    animator.SetBool("isIdle", true);
            }
            else if (isPressingW)
            {
                if (isCarrying)
                    animator.SetBool("isCarryingWalk", true);
                else
                    animator.SetBool("isWalking", true);
            }
            else
            {
                if (isCarrying)
                    animator.SetBool("isCarryingIdleWalk", true);
                else
                    animator.SetBool("isIdleWalk", true);
            }
        }

        // GERAKAN FISIK
        float currentMoveSpeed = isCarrying ? moveSpeed * 0.7f : moveSpeed;

        if (isPressingW)
        {
            Vector3 moveForce = transform.forward * moveInput * currentMoveSpeed;
            rb.AddForce(moveForce, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else if (Input.GetKey(KeyCode.S) && !isMoving)
        {
            Vector3 moveForce = -transform.forward * currentMoveSpeed * 0.5f;
            rb.AddForce(moveForce, ForceMode.Acceleration);
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
            StartCoroutine(HandleDeath("Death"));
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Licin") && !isDead)
        {
            StartCoroutine(HandleDeath("SlipDeath"));
        }
    }

    private IEnumerator HandleDeath(string deathAnimationTrigger)
    {
        isDead = true;

        if (deathSFX != null && audioSource != null)
            audioSource.PlayOneShot(deathSFX);

        // Pilih animasi kematian berdasarkan apakah membawa barang
        string finalDeathTrigger = isCarryingItem ? "CarryingDeath" : deathAnimationTrigger;

        if (animator != null)
        {
            ResetAllAnimatorBools();
            animator.SetTrigger(finalDeathTrigger);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        if (playerInventory != null)
        {
            playerInventory.ClearInventory(); // Buat fungsi ini di Inventory.cs
            isCarryingItem = false;
        }

        if (animator != null)
        {
            ResetAllAnimatorBools();
            animator.SetBool("isIdle", true); // Kembali ke idle biasa
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        isDead = false;
    }

    public void PlayThrowAnimation()
    {
        if (animator != null && isCarryingItem)
        {
            animator.SetTrigger("ThrowItem");
        }
    }

    private void ResetAllAnimatorBools()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdleWalk", false);
        animator.SetBool("isBraking", false);

        animator.SetBool("isCarryingIdle", false);
        animator.SetBool("isCarryingWalk", false);
        animator.SetBool("isCarryingIdleWalk", false);
        animator.SetBool("isCarryingBraking", false);

        animator.SetBool("isDrifting", false);
    }
}
