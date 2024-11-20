using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] private LayerMask groundLayer;
    private Quaternion initialRotation;
    // Start is called before the first frame update
    void Awake()
    {
        //initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Mantener la rotación fija en cada frame porque en la animación de muerte el rayo también "se vuelta" como el propio personaje
        //transform.rotation = initialRotation;

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
