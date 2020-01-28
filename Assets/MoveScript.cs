using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour
{
    public Animator animator;
    public float jumpForce = 500f;
    public float moveSpeedPercent = 1f;
    public float gravity = 5f;
    public int numJumps = 1;
    public float doubleJumpMultiplier = 1.5f;
    public float airControl = 5f;

    public ParticleSystem speedParticles;
    public ParticleSystem jumpParticles;

    public Image speedTimer;
    public Image jumpTimer;

    private float currentMoveSpeed;
    private bool grounded = false;
    private int currentNumJumps;
    private int jumpsLeft;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        animator.speed = moveSpeedPercent;
        currentNumJumps = numJumps;
        jumpsLeft = numJumps;

    }

    // Update is called once per frame
    void Update()
    {
        grounded = AmGrounded();
        animator.SetBool("Grounded", grounded);
        
        if(!grounded)
        {
            AirControl();
        }
        
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetButtonDown("AttackL"))
        {
            animator.SetTrigger("AttackTrigger");
        }


        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                new Quaternion(transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, Camera.main.transform.rotation.w)
                * Quaternion.LookRotation(movement), 0.1f);
        }

        animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"),Vector3.Distance(transform.position + movement * 3, transform.position), 0.1f));
        
            GetComponent<Rigidbody>().AddForce(Vector3.down * gravity);
    }

    void AirControl()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * Mathf.Abs((Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * airControl));
    }

    private void FixedUpdate()
    {
        animator.SetFloat("y", GetComponent<Rigidbody>().velocity.y);
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
        return Physics.OverlapSphere(transform.position, 0.2f).Length > 1;
    }

    bool CanJump()
    {
        if( jumpsLeft == numJumps && AmGrounded() && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            return true;
        }
        else if(jumpsLeft > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
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
}
