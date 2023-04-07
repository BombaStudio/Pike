using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform target;
    public GameObject sword;
    public float minimumDistance;
    public float maximumDistance;
    public float EnemySpeed;
    public float attackCooldown;
    public float attackDuration;
    float lastAttack;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) > maximumDistance)
        {
            rb.velocity = Vector2.zero;
        }
        else if (Vector2.Distance(transform.position, target.position) > minimumDistance)
        {
            EnemySpeed = 5;
            transform.position = Vector2.MoveTowards(transform.position, target.position, EnemySpeed * Time.deltaTime);
        }
        else
        {
            attack();
        }
    }

    public void attack()
    {
        if (Time.time - lastAttack < attackCooldown)
        {
            return;
        }
        lastAttack = Time.time;
        sword.SetActive(true);
        StartCoroutine(DeactivateSword());
    }
    IEnumerator DeactivateSword()
    {
        yield return new WaitForSeconds(attackDuration);
        sword.SetActive(false);
    }
}