using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour
{
    public Animator animator;
    public new Rigidbody rigidbody;

    public LayerMask layerMask;

    public float jumpForce = 500f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float moveSpeedPercent = 1f;
    public float gravity = 5f;
    public int numJumps = 1;
    public float doubleJumpMultiplier = 1.5f;
    public float airControl = 5f;

    public bool allowMovement = true;
    public bool allowJump = true;
    public bool turnToCamera = true;

    public ParticleSystem speedParticles;
    public ParticleSystem jumpParticles;

    public Image speedTimer;
    public Image jumpTimer;
    
    private bool grounded = false;
    private int currentNumJumps;
    private int jumpsLeft;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        animator.speed = moveSpeedPercent;

        rigidbody = GetComponent<Rigidbody>();

        currentNumJumps = numJumps;
        jumpsLeft = numJumps;

    }

    // Update is called once per frame
    void Update()
    {

        grounded = AmGrounded();

        if (!grounded)
        {
            AirControl();
        }

        Vector3 movement = Vector3.zero;
        if (allowMovement)
        {
            animator.SetBool("Grounded", grounded);
            movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (Input.GetButtonDown("AttackL"))
            {
                animator.SetTrigger("AttackTrigger");
                foreach (Collider collider in Physics.OverlapBox(GetComponent<CapsuleCollider>().bounds.center, GetComponent<CapsuleCollider>().bounds.extents, transform.rotation, 1, QueryTriggerInteraction.Collide))
                {
                    if(collider.GetComponent<Button>() && grounded)
                    {
                        collider.GetComponent<Button>().HitButton(this);
                    }
                }
            }
            if (Input.GetButtonDown("Jump") && allowJump)
            {
                Jump();
            }

            if (movement != Vector3.zero && turnToCamera)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    new Quaternion(transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, Camera.main.transform.rotation.w)
                    * Quaternion.LookRotation(movement), 0.1f);
            }
            animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"), Vector3.Distance(transform.position + movement * 3, transform.position), 0.1f));
            animator.SetFloat("y", GetComponent<Rigidbody>().velocity.y);
        }

        if (rigidbody.velocity.y < 0)
        {
            rigidbody.velocity += Vector3.up * -gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rigidbody.velocity += Vector3.up * -gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        rigidbody.velocity += Vector3.up * -gravity *  Time.deltaTime;
    }

    void AirControl()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * Mathf.Abs((Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * airControl));
    }

    private void FixedUpdate()
    {
    }


    void Jump()
    {
        if(CanJump())
        {
            animator.SetTrigger("JumpTrigger");
            GetComponent<Rigidbody>().AddForce((Vector3.up * jumpForce * (jumpsLeft < numJumps ? doubleJumpMultiplier : 1f)) + transform.forward * Input.GetAxis("Vertical") * 2, ForceMode.Impulse);
            jumpsLeft--;
        }
    }

    bool AmGrounded()
    {
        bool test = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size/2, transform.rotation, layerMask, QueryTriggerInteraction.Ignore).Length > 2;
        return test;
    }

    bool CanJump()
    {
        if( jumpsLeft == numJumps && AmGrounded() && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            return true;
        }
        else if(jumpsLeft > 0 && rigidbody.velocity.y < 5f)
        {
            return true;
        }
            return false;
    }

    public void IncreaseSpeed(float speedPercent, float duration)
    {
        animator.speed = speedPercent;
        StartCoroutine(PowerUp(0, speedTimer, speedParticles, duration));
    }

    public void IncreaseJumpNum(int jumpNumber, float duration)
    {
        currentNumJumps = jumpNumber;
        StartCoroutine(PowerUp(1, jumpTimer, jumpParticles, duration));
    }

    IEnumerator PowerUp(int powerUpIndex, Image powerUpTimer, ParticleSystem powerUpParticles,  float duration)
    {
        float timer = duration;
        if (powerUpParticles)
        {
            powerUpParticles.Play();
        }
        if (powerUpTimer)
        {
            powerUpTimer.enabled = true;
        }
        while (timer > 0)
        {
            if (powerUpTimer)
            {
                powerUpTimer.fillAmount = timer / duration;
            }
            timer -= Time.deltaTime;
            yield return 0;
        }
        currentNumJumps = numJumps;
        if (powerUpParticles)
        {
            powerUpParticles.Stop();
        }
        if (powerUpTimer)
        {
            powerUpTimer.enabled = false;
        }
        ResetPowerUp(powerUpIndex);
        yield break;
    }

    void ResetPowerUp(int powerUp)
    {
        switch(powerUp)
        {
            case 0:
                animator.speed = moveSpeedPercent;
                break;
            case 1:
                currentNumJumps = numJumps;
                break;
        }
    }

    public void ResetJumps()
    {
        jumpsLeft = currentNumJumps;
    }

    public void ForceMoveToLocation(Vector3 dest)
    {
        StartCoroutine(MoveToDest(dest, 1.7f));
    }

    IEnumerator MoveToDest(Vector3 dest, float speed)
    {
        animator.SetFloat("z", 0.7f);
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        float timer = 0f;
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            Vector3 destFlat = dest;
            destFlat.y = transform.position.y;
            transform.LookAt(destFlat);
            Vector3 newPos = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * speed);
            transform.position = newPos;
            if((timer += Time.deltaTime) > 3f)
            {
                transform.position = dest;
                break;
            }
            yield return 0;
        }
    }
}
