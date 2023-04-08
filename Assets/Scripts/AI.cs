using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    [Header("AI Parameters")]
    public bool isAlive = true;
    [Range(0,10)] public float health;
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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attack_wait = Random.Range(min_attack_wait, max_attack_wait);
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0) isAlive = false;


        if (isAlive == true)
        {
            if (Vector2.Distance(transform.position, target.position) > maximumDistance) rb.velocity = Vector2.zero;
            else if (Vector2.Distance(transform.position, target.position) > minimumDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, EnemySpeed * Time.deltaTime);

                if (transform.position.x < target.position.x) transform.localScale = new Vector2(-1, 1);
                else if (transform.position.x > target.position.x) transform.localScale = new Vector2(1, 1);
                //if (transform.position.x < target.position.x) transform.GetChild(0).localPosition = new Vector3(0.57f, 0,0);
                //else if (transform.position.x > target.position.x) transform.GetChild(0).localPosition = new Vector3(-0.57f, 0,0);
            }
            else attack();
        }
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