using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    public CinemachineCamera cinemachineCamera; //referencia a la camara
    [SerializeField] private GameObject playerHead; //referencia a la cabeza del jugador (para la lógica de la cámara y seguro es util para programar headshoots)

    private Transform currentTransform;
    private Rigidbody rb;
    [SerializeField] private float movSpeed; //velocidad de movimiento
    [SerializeField] private float movSpeedCrouched; //velocidad de movimiento agachado
    [SerializeField] private float jumpForce; //fuerza de salto, no el juego otaku
    [SerializeField] private float sensibility; //sensibilidad del raton
    private bool jumping = false; //booleano para que no salte infinito solo estando en el suelo
    private bool crouched = false; //booleano para indicar que esta agachado

    //variables de movimiento del input
    public float hor { get; set; } //movimiento horizontal
    public float ver { get; set; } //movimiento vertical

    public float horM { get; set; } //movimiento horizontal DEL RATON
    public float verM { get; set; } //movimiento vertical DEL RATON

    private Vector3 standingScale = new Vector3(0.8f,0.8f,0.8f); //escala de pie (para el collider)
    private float standingHeadHeight = 1.1f; //altura de la cabeza respecto al cuerpo de pie

    private Vector3 crouchedScale = new Vector3(0.8f, 0.55f, 0.8f); //escala agachado (para el collider)
    private float crouchedHeadHeight = 0.85f; //altura de la cabeza respecto al cuerpo agachado

    private Vector3 headMovement; //movimiento de la cabeza respecto al cuerpo del jugador (no lo hacemos hijo por temas del collider al agacharse)


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentTransform = transform;
        rb = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            transform.parent.GetComponent<PlayerInput>().enabled = true;
            cinemachineCamera = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
            cinemachineCamera.Follow = playerHead.transform;

            //deactivamos los mesh renderer nuestros para que la cámara no vea nuestro cuerpo (pero el de los demás si)
            transform.GetComponent<MeshRenderer>().enabled = false;
            playerHead.transform.GetComponent<MeshRenderer>().enabled = false;
        }

        headMovement = new Vector3(currentTransform.position.x, currentTransform.position.y + standingHeadHeight, currentTransform.position.z);
    }
    void Update()
    {
        if (crouched)
        {
            headMovement = new Vector3(currentTransform.position.x, currentTransform.position.y + crouchedHeadHeight, currentTransform.position.z);

        }
        else
        {
            headMovement = new Vector3(currentTransform.position.x, currentTransform.position.y + standingHeadHeight, currentTransform.position.z);
        }
        playerHead.transform.position = headMovement;
    }

    public void FixedUpdate()
    {
            UpdateMovement();
            UpdateMouseLook();
    }

    #region ONMOVE
    private void UpdateMovement()
    {
        Vector3 velocityX = Vector3.zero;
        float speed;
        if (crouched)
        {
            speed = movSpeedCrouched; 
        }
        else
        {
            speed = movSpeed;
        }

        if (hor != 0 || ver != 0)
        {
            Vector3 direction = (currentTransform.forward * ver + currentTransform.right * hor).normalized;
            velocityX = direction * speed;
        }

        velocityX.y = rb.velocity.y;
        rb.velocity = velocityX;
        
    }
    #endregion

    #region ONMOUSE
    private void UpdateMouseLook()
    {
        Vector2 rotation = playerHead.transform.localEulerAngles;
            if (horM != 0)
            {
                currentTransform.Rotate(0.0f, horM * (sensibility * 100) * Time.deltaTime, 0.0f);
                rotation.y = currentTransform.rotation.eulerAngles.y;
        }
            if (verM != 0)
            {
                rotation.x = (rotation.x - verM * (sensibility * 100) * Time.deltaTime + 360) % 360;
                if (rotation.x > 80 && rotation.x < 180)
                {
                    rotation.x = 80;
                }
                if (rotation.x < 280 && rotation.x > 180)
                {
                    rotation.x = 280;
                }
            }
         playerHead.transform.localEulerAngles = rotation;
    }

    #endregion

    #region ONJUMP
    public void Saltar()
    {
        SaltarServerRpc();
    }

    [ServerRpc]
    public void SaltarServerRpc()
    {
        if (!jumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    public void SetJumping(bool a)
    {
        jumping = a;
    }

    public bool GetJumping()
    {
        return jumping;
    }

    #endregion

    #region ONCROUCH
    public void EnterCrouch() {
        EnterCrouchServerRpc();
    }
    public void ExitCrouch()
    {
        ExitCrouchServerRpc();   
    }

    [ServerRpc]
    public void EnterCrouchServerRpc()
    {
        crouched = true;
        //aqui cambiamos la escala de la capsula (principalmente para que el collider se haga más bajo),
        //cuando tengamos modelos no deberia ser así a no ser que el modelo y el objeto jugador se manejen por separado
        transform.localScale = crouchedScale;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.25f, transform.localPosition.z);
    }

    [ServerRpc]
    public void ExitCrouchServerRpc()
    {
        crouched = false;
        //volvemos a cambiar la escala
        transform.localScale = standingScale;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.25f, transform.localPosition.z);
    }
    #endregion
}
