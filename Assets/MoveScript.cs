using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    public Animator animator;
    public float jumpForce = 500f;
    public float gravity = 5f;

    public float airControl = 5f;

    private bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {
        grounded = AmGrounded();
        animator.SetBool("Grounded", grounded);
        
        
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetButtonDown("AttackL") && !animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            animator.SetTrigger("AttackTrigger");
        }


        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation * Quaternion.LookRotation(movement), 0.1f);
        }

        animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"),Vector3.Distance(transform.position + movement * 3, transform.position), 0.1f));

        if (!grounded)
        {
            AirControl();
        }

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
        if(grounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            animator.SetTrigger("JumpTrigger");
            GetComponent<Rigidbody>().AddForce((Vector3.up * jumpForce) + transform.forward * Input.GetAxis("Vertical") * 2);
        }
    }

    bool AmGrounded()
    {
        return Physics.OverlapSphere(transform.position, 0.5f).Length > 1;
    }
}
