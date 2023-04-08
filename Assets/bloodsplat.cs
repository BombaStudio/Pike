using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodsplat : MonoBehaviour
{
    private ParticleSystem Bloodsplat;
    // Start is called before the first frame update
    void Start()
    {
        Bloodsplat = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Bloodsplat.Play();
        }
    }
}
