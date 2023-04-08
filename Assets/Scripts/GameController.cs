using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Parameters")]
    public string GameState;
    [SerializeField] Player player;

    [Header("Cooldown Settings")]
    [SerializeField] [Range(1, 5)] float min_spawn_cooldown;
    [SerializeField] [Range(5, 10)] float max_spawn_cooldown;
    [SerializeField] float cooldown;

    [Header("Entity Settings")]
    [SerializeField] GameObject entityPrefab;
    [SerializeField] List<Transform> spawners;

    GameObject entity;
    void Start()
    {
        
    }

    void Update()
    {
        foreach(Transform t in transform.Find("Canvas").GetComponentInChildren<Transform>())
        {
            if (t.name == GameState) t.gameObject.SetActive(true);
            else t.gameObject.SetActive(false);
        }
        switch (GameState)
        {
            case "Game":
                for (int i = 1; i < 5; i++)
                {
                    if (i > player.health) transform.Find("Canvas").Find("Game").Find("heat" + i.ToString()).GetComponent<Image>().color = Color.black;
                    else transform.Find("Canvas").Find("Game").Find("heat" + i.ToString()).GetComponent<Image>().color = Color.red;
                }
                if (cooldown > 0) cooldown -= Time.deltaTime;
                else
                {
                    if (spawners.Count > 0)
                    {
                        entity = Instantiate(entityPrefab, spawners[Random.Range(0,spawners.Count)].position,Quaternion.identity);;
                        entity.GetComponent<AI>().target = player.transform;
                    }
                    cooldown = Random.Range(min_spawn_cooldown, max_spawn_cooldown);
                }
                Time.timeScale = 1;
                break;
            case "Pause":
                Time.timeScale = 0;
                break;
            default: break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (spawners.Count > 0)
        {
            foreach (Transform s in spawners)
            {
                if (s) Gizmos.DrawWireCube(s.position, new Vector3(1, 1, 1));
            }
        }
    }
}
