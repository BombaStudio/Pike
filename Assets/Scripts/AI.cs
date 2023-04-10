using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    [Header("AI Parameters")]
    public bool isAlive = true;
    [Range(0,10)] public int health = 4;
    [Range(0, 1)] public int entity_id;
    public float EnemySpeed;
    public float EnemyJumpForce;
    public Transform target;
    public GameObject sword;
    
    [Header("Distance")]
    public float minimumDistance;
    public float maximumDistance;
    
    [Header("Cooldowns")]
    public float attackCooldown;
    public float attackDuration;

    [SerializeField] [Range(0.1f, 0.5f)] float min_attack_wait;
    [SerializeField] [Range(0.5f, 1.0f)] float max_attack_wait;

    [SerializeField] float attack_wait;


    float lastAttack;
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attack_wait = Random.Range(min_attack_wait, max_attack_wait);
    }

    void Update()
    {
        GetComponent<Animator>().SetBool("live", isAlive);
        GetComponent<Animator>().SetInteger("id", entity_id);

        if (health <= 0) isAlive = false;



        if (isAlive == true)
        {
            if (entity_id == 0)
            {
                if (Vector2.Distance(transform.position, target.position) > maximumDistance) rb.velocity = new Vector2(Vector2.zero.x, Physics.gravity.y);
                else if (Vector2.Distance(transform.position, target.position) > minimumDistance)
                {
                    if (Mathf.Abs(transform.position.y - target.position.y) > 1 && target.position.y < transform.position.y) new Vector2(Vector2.zero.x, Physics.gravity.y);
                    else
                    {
                        transform.position = Vector2.MoveTowards(transform.position, target.position, EnemySpeed * Time.deltaTime);

                        if (transform.position.x < target.position.x) transform.localScale = new Vector2(-1, 1);
                        else if (transform.position.x > target.position.x) transform.localScale = new Vector2(1, 1);
                        //if (transform.position.x < target.position.x) transform.GetChild(0).localPosition = new Vector3(0.57f, 0,0);
                        //else if (transform.position.x > target.position.x) transform.GetChild(0).localPosition = new Vector3(-0.57f, 0,0);
                    }
                }
                else attack();
            }
            else if (entity_id == 1)
            {
                if (Vector2.Distance(transform.position, target.position) > maximumDistance) rb.velocity = Vector2.zero;
                else if (Vector2.Distance(transform.position, target.position) > minimumDistance)
                {
                    //EnemySpeed = 5;
                    transform.position = Vector2.MoveTowards(transform.position, target.position, EnemySpeed * Time.deltaTime);

                    if (transform.position.x < target.position.x) transform.localScale = new Vector2(-1, 1);
                    else if (transform.position.x > target.position.x) transform.localScale = new Vector2(1, 1);
                }
                else attack();
            }

            if (entity_id == 0) rb.gravityScale = 1;
            else rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void LateUpdate()
    {

    }

    public void attack()
    {
        if (Time.time - lastAttack < attackCooldown) return;

        if (attack_wait > 0) attack_wait -= Time.deltaTime;
        else
        {
            lastAttack = Time.time;
            sword.SetActive(true);
            StartCoroutine(DeactivateSword());
            GetComponent<AudioSource>().Play();
            attack_wait = Random.Range(min_attack_wait, max_attack_wait);
        }
    }
    IEnumerator DeactivateSword()
    {
        yield return new WaitForSeconds(attackDuration);
        sword.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maximumDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumDistance);
    }
}