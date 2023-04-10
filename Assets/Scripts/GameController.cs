using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Parameters")]
    public string GameState;
    [SerializeField] Player player;
    [SerializeField] Canvas InGameCanvas;
    public int score;

    [Header("Cooldown Settings")]
    [SerializeField] [Range(1, 5)] float min_spawn_cooldown;
    [SerializeField] [Range(5, 10)] float max_spawn_cooldown;
    [SerializeField] float cooldown;

    [Header("Entity Settings")]
    [SerializeField] GameObject entityPrefab;
    [SerializeField] List<Transform> spawners;

    GameObject entity;

    Vector3 startPos;
    void Start()
    {
        startPos = player.transform.position;
        
    }

    void Update()
    {
        InGameCanvas.transform.GetChild(0).GetComponent<Text>().text = score.ToString();
        foreach(Transform t in transform.Find("Canvas").GetComponentInChildren<Transform>())
        {
            if (t.name == GameState) t.gameObject.SetActive(true);
            else t.gameObject.SetActive(false);
        }
        switch (GameState)
        {
            case "Game":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                InGameCanvas.enabled = true;
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
                        int s = Random.Range(0, spawners.Count);
                        entity = Instantiate(entityPrefab, spawners[s].position,Quaternion.identity);
                        entity.GetComponent<AI>().entity_id = Random.Range(0,2);
                        entity.GetComponent<AI>().target = player.transform;
                        entity.GetComponent<AI>().health = 4;
                        entity.GetComponent<AI>().isAlive = true;
                    }
                    cooldown = Random.Range(min_spawn_cooldown, max_spawn_cooldown);
                }

                if (Input.GetKeyDown(KeyCode.Escape)) ChangeState("Pause");
                Time.timeScale = 1;
                break;
            case "Pause":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                break;
            case "GameOver":
                InGameCanvas.enabled = false;
                player.health = 4;
                score = 0;
                player.transform.position = startPos;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                break;
            case "Start":
                InGameCanvas.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                break;
            default: break;
        }
    }

    public void ChangeState(string state)
    {
        GameState = state;
        GetComponent<AudioSource>().Play();
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
