using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public CinemachineCamera cinemachineCamera; //referencia a la camara

    private Transform currentTransform;
    private Rigidbody rb;
    [SerializeField] private float movSpeed; //velocidad de movimiento
    [SerializeField] private float movSpeedCrouched; //velocidad de movimiento agachado
    [SerializeField] private float jumpForce; //fuerza de salto, no el juego otaku
    [SerializeField] private float sensibility; //sensibilidad del raton
    private bool jumping = false; //booleano para que no salte infinito solo estando en el suelo
    private bool crouched = false; //booleano para indicar que esta agachado

    //variables de movimiento del input
    public float hor; //movimiento horizontal
    public float ver; //movimiento vertical

    public float horM; //movimiento horizontal DEL RATON
    public float verM; //movimiento vertical DEL RATON

    private Vector3 standingScale = new Vector3(1,1,1); //escala de pie (para el collider)
    private Vector3 standingCameraOffset = new Vector3(0,0.8f,0.3f); //offset de la cámara de pie (posición aproximada de la cabeza)

    private Vector3 crouchedScale = new Vector3(1, 0.6f, 1); //escala agachado (para el collider)
    private Vector3 crouchedCameraOffset = new Vector3(0, 0.48f, 0.3f); //offset de la cámara agachado (posición aproximada de la cabeza)


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
            cinemachineCamera.Follow = transform;
        }
    }
    void Update()
    {
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
        if (IsOwner)
        {
            Vector2 rotation = cinemachineCamera.transform.localEulerAngles;
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
            cinemachineCamera.transform.localEulerAngles = rotation;
        }
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
        //if (!jumping)
        //{
        crouched = true;
        //aqui cambiamos la escala de la capsula (principalmente para que el collider se haga más bajo),
        //cuando tengamos modelos no deberia ser así a no ser que el modelo y el objeto jugador se manejen por separado
        transform.localScale = crouchedScale;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.4f, transform.localPosition.z);
        cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = crouchedCameraOffset;
        //}   
    }

    [ServerRpc]
    public void ExitCrouchServerRpc()
    {
        crouched = false;
        //volvemos a cambiar la escala
        transform.localScale = standingScale;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.4f, transform.localPosition.z);
        cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = standingCameraOffset;
    }
    #endregion
}
