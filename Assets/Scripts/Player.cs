using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [Range(0, 4)] public int health;
    public float moveSpeed = 5f;
    public float jumpForce = 7.5f;


    [Header("Gravity Settings")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    [Header("Camera Settings")]
    [SerializeField] float mincameradistance,maxcameradistance = 5;
    [SerializeField] bool camMove;

    

    [Header("Demage Settings")]
    [SerializeField] bool demaged;
    [SerializeField] LayerMask demageLayer;

    [Header("Attack Settings")]
    [SerializeField] [Range(1,10)] float attackDistance = 3;
    [SerializeField] [Range(0.1f, 0.2f)] float min_attack_wait;
    [SerializeField] [Range(0.2f, .03f)] float max_attack_wait;
    [SerializeField] float attack_wait;
    [SerializeField] Vector3 attack_dir;
    [SerializeField] LayerMask attackLayer;

    private bool isGrounded;
    private Rigidbody2D rb;
    //private Animator animator;
    float pain = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        attack_dir = transform.Find("Sword").transform.localPosition;
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        if (moveInput < 0)
        {
            //transform.eulerAngles = new Vector3(0, 180, 0);
            attack_dir = new Vector2(-.55f,0);
        }
        else if (moveInput > 0)
        {
            //transform.eulerAngles = new Vector3(0, 0, 0);
            attack_dir = new Vector2(.55f,0);
        }
        transform.Find("Sword").transform.localPosition = attack_dir;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);


        if (pain > 0) pain -= Time.deltaTime;
        else
        {
            if (Physics2D.OverlapCircle(groundCheck.position, checkRadius, demageLayer))
            {
                health -= 1;
                pain = 0.2f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if (attack_wait > 0) attack_wait -= Time.deltaTime;
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D attack = Physics2D.OverlapCircle(transform.Find("Sword").position, attackDistance, attackLayer);
                if (attack)
                {
                    attack.GetComponent<AI>().isAlive = false;
                    attack_wait = Random.Range(min_attack_wait, max_attack_wait);
                }
            }
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.Find("Sword").transform.position, attackDistance);
    }
}
