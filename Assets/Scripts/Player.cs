using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float shakeDuration = 0.2f;
    public float shakeAmount = 0.2f;

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

    [Header("GameController")]
    [SerializeField] GameController controller;

    private bool isGrounded;
    private Rigidbody2D rb;
    //private Animator animator;
    float pain = 0;

    [SerializeField] bool highShake;

    private Vector3 originalPos;

    Vector3 scl;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        attack_dir = transform.Find("Sword").transform.localPosition;
        scl = transform.localScale;
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        if (moveInput < 0)
        {
            //transform.eulerAngles = new Vector3(0, 180, 0);
            //attack_dir = new Vector2(-.55f,0);
            transform.localScale = new Vector3(-scl.x, scl.y, 1);
        }
        else if (moveInput > 0)
        {
            //transform.eulerAngles = new Vector3(0, 0, 0);
            //attack_dir = new Vector2(.55f,0);
            transform.localScale = new Vector3(scl.x, scl.y, 1);
        }
        transform.Find("Sword").transform.localPosition = attack_dir;
    }

    private void Update()
    {
        if (health <= 0) controller.GameState = "GameOver";

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);


        if (pain > 0) pain -= Time.deltaTime;
        else
        {
            if (Physics2D.OverlapCircle(groundCheck.position, checkRadius, demageLayer))
            {
                health -= 1;
                transform.Find("Bloodsplat").GetComponent<ParticleSystem>().Play();
                pain = 0.2f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity = -Vector2.up * jumpForce * 4;
            highShake = true;
        }

        if (attack_wait > 0)
        {
            attack_wait -= Time.deltaTime;
            transform.Find("Sword").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Sword").gameObject.SetActive(false);
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D attack = Physics2D.OverlapCircle(transform.Find("Sword").position, attackDistance, attackLayer);
                if (attack && attack.GetComponent<AI>().health > 0)
                {
                    attack.GetComponent<AI>().health -= 1;
                    GetComponent<AudioSource>().Play();
                    attack.transform.Find("Bloodsplat").GetComponent<ParticleSystem>().Play();
                    if (attack.GetComponent<AI>().health <= 0) controller.score++;
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
            originalPos = Camera.main.transform.position;
            if (Vector2.Distance(transform.position, Camera.main.transform.position) < mincameradistance)
            {
                camMove = false;
            }
        }
        if (isGrounded && highShake)
        {
            Shake();
        }
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + new Vector3(Random.Range(-1f, 1f) * shakeAmount, Random.Range(-1f, 1f) * shakeAmount, -10), 10);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
        highShake = false;
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
