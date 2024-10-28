using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] private LayerMask groundLayer;
    // Start is called before the first frame update
    void Awake()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundStatus();
    }

    void CheckGroundStatus()
    {

        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * 0.5f);

        if (Physics.Raycast(landingRay, out hit, 0.5f, groundLayer))
        {
            playerController.SetJumping(false);
        }
        else
        {
            playerController.SetJumping(true);
        }
    }

}
