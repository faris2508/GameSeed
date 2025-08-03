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

    public float driftTurnMultiplier = 1.5f;
    public float driftDrag = 0.8f;
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    public ParticleSystem driftParticles; // Particle untuk drift
    public ParticleSystem brakeParticles; // Particle untuk rem

    private bool isDead = false;
    [SerializeField] private Transform model;
    [Header("SFX")]
    public AudioSource audioSource; // Audio Source untuk SFX
    public AudioClip deathSFX; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.linearDamping = 0.1f;
        rb.useGravity = true;

        // Bekukan semua rotasi, kita atur rotasi manual lewat transform.Rotate()
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Reset rotasi fisika agar tidak muter liar
        rb.angularVelocity = Vector3.zero;

        float turnInput = Input.GetAxis("Horizontal");
        float moveInput = Input.GetAxis("Vertical");
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        bool isPressingW = moveInput > 0;

        // DRIFT
        if (Input.GetKey(KeyCode.LeftShift) && isMoving)
        {
            transform.Rotate(0, turnInput * turnSpeed * driftTurnMultiplier * Time.fixedDeltaTime, 0);
            rb.linearDamping = driftDrag;
            animator.SetBool("isDrifting", true);

            // Aktifkan drift particle
            if (driftParticles != null && !driftParticles.isPlaying) driftParticles.Play();
        }
        else
        {
            transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
            animator.SetBool("isDrifting", false);

            // Matikan drift particle
            if (driftParticles != null && driftParticles.isPlaying) driftParticles.Stop();
        }

        if (Input.GetKey(KeyCode.S) && rb.linearVelocity.magnitude > 0.5f && !Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearDamping = brakeDrag;
            animator.SetBool("isBraking", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdleWalk", false);
            animator.SetBool("isIdle", false);

        // Aktifkan brake particle
        if (brakeParticles != null && !brakeParticles.isPlaying) brakeParticles.Play();
        }
        else if (Input.GetKey(KeyCode.S) && rb.linearVelocity.magnitude <= 0.5f) // Tekan S tapi diam
        {
            rb.linearDamping = 0.1f;
            animator.SetBool("isBraking", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdleWalk", false);
            animator.SetBool("isIdle", true);

            // Matikan brake particle
            if (brakeParticles != null && brakeParticles.isPlaying) brakeParticles.Stop();
        }
        else
        {
        rb.linearDamping = 0.1f;
        animator.SetBool("isBraking", false);

        // Matikan brake particle
        if (brakeParticles != null && brakeParticles.isPlaying) brakeParticles.Stop();

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

        // GERAKAN MAJU & MUNDUR
        if (isPressingW)
        {
            Vector3 moveForce = transform.forward * moveInput * moveSpeed;
            rb.AddForce(moveForce, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else if (Input.GetKey(KeyCode.S) && !isMoving)
        {
            Vector3 moveForce = -transform.forward * moveSpeed * 0.5f;
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

        if (animator != null)
            animator.SetTrigger(deathAnimationTrigger);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        animator.SetTrigger("Idle");

        // Kembali freeze rotasi penuh
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        isDead = false;
    }
}
