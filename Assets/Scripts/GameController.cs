using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public string GameState;
    [SerializeField] Player player;
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
                Time.timeScale = 1;
                break;
            case "Pause":
                Time.timeScale = 0;
                break;
            default: break;
        }
    }
}
