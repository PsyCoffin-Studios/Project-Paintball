using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngine.Animations;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public CinemachineCamera cinemachineCamera; //referencia a la camara
    [SerializeField] GameObject playerHead; //referencia a la cabeza del jugador (para la lógica de la cámara y seguro es util para programar headshoots)


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

    private Vector3 standingHeight = new Vector3(0f,1.1f,0f); //altura de pie (para el collider)
    private Vector3 crouchedHeight = new Vector3(0f, 0.6f, 0f); //altura agachado (para el collider)

    private Vector3 colliderStandingHeight = new Vector3(0f, 0.55f, 0f); //altura de pie (para el collider)
    private Vector3 colliderCrouchedHeight = new Vector3(0f, 0.05f, 0f); //altura agachado (para el collider)


    public Transform _camTransform;

    public GameObject mobileChecker;
    public GameObject textDebug;


    //GameObject model;
    public GameObject selectedGun;
    [SerializeField]Animator animator;

    // prefabs por si usa granadas
    public GameObject boteHumoPrefab; // Prefab del bote de humo
    public GameObject esferaHumoPrefab; // Prefab de la esfera de humo
    public GameObject granadaPrefab; // Prefab del bote de humo
    public GameObject pinturaPrefab; // Prefab de la esfera de humo

    [SerializeField] private BoxCollider pecho;

    private NetworkVariable<float> walkingState = new NetworkVariable<float>( // Nombre del jugador
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    void OnWalkingStateChanged(float previous, float newValue)
    {
        if (walkingState.Value > -0.5f && walkingState.Value < 0.5f)
        {
            animator.SetFloat("speed",0);
        }else if (walkingState.Value > 0.5f)
        {
            animator.SetFloat("speed", 1);
        }
        else if (walkingState.Value < -0.5f)
        {
            animator.SetFloat("speed", -1);
        }
    }

    private void Awake()
    {
        //nombrePersonaje.Value = DataBetweenScenes.instance.GetNombre();
        //arma.Value = DataBetweenScenes.instance.GetArma();
        //hab.Value = DataBetweenScenes.instance.GetHabilidad();

        //Aqui  habr a que actualizar la habilidad, el arma y la skin!!!!!!!!!!!!!!!!!!!!!!!!
        //InitHabilidad();

    }


    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        currentTransform = transform;
        rb = GetComponent<Rigidbody>();
        walkingState.OnValueChanged += OnWalkingStateChanged;

        //face = transform.Find("face");

        cinemachineCamera = _camTransform.GetComponent<CinemachineCamera>();

        GameObject.Find("@UIManager").GetComponent<UIControllerInGame>().ActualizarDatosNetcode();

        if (IsOwner)
        {
            mobileChecker.SetActive(true);
            textDebug.SetActive(true);


            transform.GetComponent<PlayerInput>().enabled = true;
            cinemachineCamera.Follow = playerHead.transform;

            cinemachineCamera.Priority = 1; //Asignamos la prioridad de la cámara para que solo la coja el propietario

            
        }
        else
        {
            cinemachineCamera.Priority = 0;

            if (this.cinemachineCamera == null)
            {
                Debug.LogError("No se encontró la CinemachineCamera para este cliente (PLAYER CONTROLER).");
                return; // Sale si no encuentra la cámara
            }
        }

    }
    public override void OnNetworkDespawn()
    {
        walkingState.OnValueChanged -= OnWalkingStateChanged;
    }


    void Update()
    {
    }

    public void FixedUpdate()
    {
        if (!IsServer) { return; }
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
            walkingState.Value = 1f;
        }
        else
        {
            walkingState.Value = 0f;
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
                currentTransform.Rotate(0.0f, horM * (sensibility), 0.0f);
                //rotation.y = currentTransform.rotation.eulerAngles.y;
        }
            if (verM != 0)
            {
                rotation.x = (rotation.x - verM * (sensibility) + 360) % 360;
                if (rotation.x > 30 && rotation.x < 180)
                {
                    rotation.x = 30;
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
            //rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    public void SetJumping(bool a)
    {
        jumping = a;
        animator.SetBool("jumping", a);
    }

    public bool GetJumping()
    {
        return jumping;
    }

    public void SetAnimator(Animator a)
    {
        animator = a;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void SetPlayerHead(GameObject a)
    {
        playerHead = a;
    }

    public GameObject GetPlayerHead()
    {
        return playerHead;
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
        animator.SetBool("crouched", true);
        //playerHead.transform.localPosition = crouchedHeight;
        //pecho.center = colliderCrouchedHeight;
    }

    [ServerRpc]
    public void ExitCrouchServerRpc()
    {
        crouched = false;
        animator.SetBool("crouched", false);
        //playerHead.transform.localPosition = standingHeight;
        //pecho.center = colliderStandingHeight;
    }
    #endregion

    #region SENSIBILIDAD
    public void SetSensibility(float newSensibility)
    {
        sensibility = newSensibility;
    }

    public float GetSensibility()
    {
        return sensibility;
    }
    #endregion
}
