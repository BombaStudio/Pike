using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7.5f;

    private Rigidbody2D rb;
    //private Animator animator;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    [SerializeField] float mincameradistance,maxcameradistance = 5;
    [SerializeField] bool camMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        //animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        //animator.SetBool("Grounded", isGrounded);
    }
    private void LateUpdate()
    {
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > maxcameradistance && !camMove) camMove = true;
        
        if (camMove)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z), Time.deltaTime);
            if (Vector2.Distance(transform.position, Camera.main.transform.position) < mincameradistance)
            {
                camMove = false;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position,checkRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxcameradistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,mincameradistance);
    }
}
