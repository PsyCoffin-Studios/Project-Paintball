using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //comprobar si el jugador está en el suelo para saltar
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("ashsjkdasg");
        if (collision.gameObject.CompareTag("Suelo"))
        {
            playerController.SetJumping(false);
        }
    }
    //comprobar que el jugador ha saltado o está cayendo de alguna superficie para no poder saltar en el aire
    void OnTriggerExit(Collider collision)
    {
        Debug.Log("ashsjkdasg    fdsfsdfsd");
        if (collision.gameObject.CompareTag("Suelo"))
        {
            playerController.SetJumping(true);
        }
    }
}
