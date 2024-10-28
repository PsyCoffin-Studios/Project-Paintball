using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Collections;

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
    public float hor; //movimiento horizontal
    public float ver; //movimiento vertical

    public float horM; //movimiento horizontal DEL RATON
    public float verM; //movimiento vertical DEL RATON

    private Vector3 standingScale = new Vector3(0.8f,0.8f,0.8f); //escala de pie (para el collider)
    private float standingHeadHeight = 1.1f; //altura de la cabeza respecto al cuerpo de pie

    private Vector3 crouchedScale = new Vector3(0.8f, 0.55f, 0.8f); //escala agachado (para el collider)
    private float crouchedHeadHeight = 0.65f; //altura de la cabeza respecto al cuerpo agachado

    private Vector3 headMovement; //movimiento de la cabeza respecto al cuerpo del jugador (no lo hacemos hijo por temas del collider al agacharse)

    public Transform _camTransform;

    //VARIABLES DE PERSONAJE Y PISTOLA// (AÑADIR VELOCIDAD Y OTRAS COSAS FACILMENTE!!!!!!!!!!!!!

    //////////DATOS COMPARTIDOS ENTRE ESCENAS PARA SABER EL PERSONAJE Y EL ARMA//////////
    //string nombrePersonaje;
    //string arma;
    //string hab;

    private NetworkVariable<FixedString128Bytes> nombrePersonaje = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "nombre",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<FixedString128Bytes> arma = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "arma",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<FixedString128Bytes> hab = new NetworkVariable<FixedString128Bytes>( // Nombre del jugador
        "hab",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);



    // prefabs por si usa granadas
    public GameObject boteHumoPrefab; // Prefab del bote de humo
    public GameObject esferaHumoPrefab; // Prefab de la esfera de humo
    public GameObject granadaPrefab; // Prefab del bote de humo
    public GameObject pinturaPrefab; // Prefab de la esfera de humo

    // escribir el tipo de habilidad que tendra el player
    private IHabilidad habilidad;



    Animator animator;
    public GameObject[] personajes;
    public GameObject[] armas;

    GameObject model;
    public GameObject selectedGun;

    public GameObject mobileChecker;

    private void Awake()
    {
        //nombrePersonaje.Value = DataBetweenScenes.instance.GetNombre();
        //arma.Value = DataBetweenScenes.instance.GetArma();
        //hab.Value = DataBetweenScenes.instance.GetHabilidad();

        Debug.LogWarning($"He elegido el personaje de {nombrePersonaje} y tengo un/a {arma}");


        //Aqui  habr a que actualizar la habilidad, el arma y la skin!!!!!!!!!!!!!!!!!!!!!!!!
        //InitHabilidad();

    }


    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        
        ApplyInfo(nombrePersonaje.Value,arma.Value,hab.Value);

        nombrePersonaje.OnValueChanged += OnNameChanged;
        arma.OnValueChanged += OnArmaChanged;
        hab.OnValueChanged += OnHabChanged;
        
        SetInfo(DataBetweenScenes.instance.GetNombre(), DataBetweenScenes.instance.GetArma(), DataBetweenScenes.instance.GetHabilidad());

        currentTransform = transform;
        rb = GetComponent<Rigidbody>();

        headMovement = new Vector3(currentTransform.position.x, currentTransform.position.y + standingHeadHeight, currentTransform.position.z);
        cinemachineCamera = _camTransform.GetComponent<CinemachineCamera>();

        GameObject.Find("@UIManager").GetComponent<UIControllerInGame>().ActualizarDatosNetcode();

        if (IsOwner)
        {
            mobileChecker.SetActive(true);
            
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
        animator.SetFloat("speed", 0);
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
            animator.SetFloat("speed", 1);
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
        animator.SetBool("crouched",true);
        //aqui cambiamos la escala de la capsula (principalmente para que el collider se haga más bajo),
        //cuando tengamos modelos no deberia ser así a no ser que el modelo y el objeto jugador se manejen por separado
        //transform.localScale = crouchedScale;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.25f, transform.localPosition.z);
    }

    [ServerRpc]
    public void ExitCrouchServerRpc()
    {
        crouched = false;
        animator.SetBool("crouched", false);
        //volvemos a cambiar la escala
        //transform.localScale = standingScale;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.25f, transform.localPosition.z);
    }
    #endregion




    // habilidad del player
    public void UsarHabilidad()
    {
        if (IsOwner)
        {
            habilidad.Use(this);
        }
        //UsarHabilidadServerRpc();
    }

    [ServerRpc]
    public void UsarHabilidadServerRpc()
    {
        //habilidad.Use(this);
    }


    // para no meter todo el textazo en Awake
    private void InitHabilidad()
    {
        switch (hab.Value.ToString()) {
            case "Dash":

                habilidad = new DashHab();
                break;
            case "Invisibilidad":
                habilidad = new InvisibleHab();
                break;
            case "Explosiones":
                habilidad = new ExplosionHab();
                break;
            case "BombasDeHumo":
                habilidad = new SmokeHab();
                break;
        }
    }


    //network variable
    public override void OnNetworkDespawn()
    {
        nombrePersonaje.OnValueChanged -= OnNameChanged;
        arma.OnValueChanged -= OnArmaChanged;
        hab.OnValueChanged -= OnHabChanged;
    }
    void OnNameChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        //nombrePersonaje.Value = newValue;
        switch (nombrePersonaje.Value.ToString())
        {
            case "Hex":
                personajes[0].SetActive(true);
                personajes[1].SetActive(false);
                personajes[2].SetActive(false);
                personajes[3].SetActive(false);
                animator = personajes[0].GetComponent<Animator>();
                break;
            case "Joker":
                personajes[1].SetActive(true);
                personajes[0].SetActive(false);
                personajes[2].SetActive(false);
                personajes[3].SetActive(false);
                animator = personajes[1].GetComponent<Animator>();
                break;
            case "Revenant":
                personajes[2].SetActive(true);
                personajes[0].SetActive(false);
                personajes[1].SetActive(false);
                personajes[3].SetActive(false);
                animator = personajes[2].GetComponent<Animator>();
                break;
            case "Outcast":
                personajes[3].SetActive(true);
                personajes[0].SetActive(false);
                personajes[1].SetActive(false);
                personajes[2].SetActive(false);
                animator = personajes[3].GetComponent<Animator>();
                break;
        }
    }
    void OnArmaChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        //arma.Value = newValue;
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());

                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());

                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());

                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());

                    break;
            }
        }
    }
    void OnHabChanged(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        switch (hab.Value.ToString())
        {
            case "Dash":

                habilidad = new DashHab();
                break;
            case "Invisibilidad":
                habilidad = new InvisibleHab();
                break;
            case "Explosiones":
                habilidad = new ExplosionHab();
                break;
            case "BombasDeHumo":
                habilidad = new SmokeHab();
                break;
        }
    }

    public void SetInfo(FixedString128Bytes newValueN, FixedString128Bytes newValueW, FixedString128Bytes newValueH)
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            SetInfoServerRpc(newValueN,newValueW,newValueH);
        }
    }

    [ServerRpc]
    private void SetInfoServerRpc(FixedString128Bytes newValueN, FixedString128Bytes newValueW, FixedString128Bytes newValueH)
    {
        nombrePersonaje.Value = newValueN;
        arma.Value = newValueW;
        hab.Value = newValueH;
    }


    // Aplicamos a cada coche su nombre cuando se conecten nuevos para que todos los jugadores puedan ver el nombre del resto
    private void ApplyInfo(FixedString128Bytes textName, FixedString128Bytes textWeapon, FixedString128Bytes textHability)
    {
        if (nombrePersonaje != null)
        {
            //nombrePersonaje.Value = textName;
            switch (nombrePersonaje.Value.ToString())
            {
                case "Hex":
                    personajes[0].SetActive(true);
                    personajes[1].SetActive(false);
                    personajes[2].SetActive(false);
                    personajes[3].SetActive(false);
                    animator = personajes[0].GetComponent<Animator>();
                    break;
                case "Joker":
                    personajes[1].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[2].SetActive(false);
                    personajes[3].SetActive(false);
                    animator = personajes[1].GetComponent<Animator>();
                    break;
                case "Revenant":
                    personajes[2].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[1].SetActive(false);
                    personajes[3].SetActive(false);
                    animator = personajes[2].GetComponent<Animator>();
                    break;
                case "Outcast":
                    personajes[3].SetActive(true);
                    personajes[0].SetActive(false);
                    personajes[1].SetActive(false);
                    personajes[2].SetActive(false);
                    animator = personajes[3].GetComponent<Animator>();
                    break;
            }

            
        }
        if (arma != null)
        {
            //arma.Value = textWeapon;
            switch (arma.Value.ToString())
            {
                case "Pistola":
                    armas[0].SetActive(true);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[0];
                    GetComponent<InputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[0].GetComponent<WeaponController>());
                    break;
                case "Rifle":
                    armas[1].SetActive(true);
                    armas[0].SetActive(false);
                    armas[2].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[1];
                    GetComponent<InputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[1].GetComponent<WeaponController>());
                    break;
                case "Escopeta":
                    armas[2].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[3].SetActive(false);
                    //selectedGun = armas[2];
                    GetComponent<InputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[2].GetComponent<WeaponController>());
                    break;
                case "Francotirador":
                    armas[3].SetActive(true);
                    armas[0].SetActive(false);
                    armas[1].SetActive(false);
                    armas[2].SetActive(false);
                    //selectedGun = armas[3];
                    GetComponent<InputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    GetComponent<MobileInputController>().SetGun(armas[3].GetComponent<WeaponController>());
                    break;
            }
        }
        if (hab != null)
        {
            switch (hab.Value.ToString())
            {
                case "Dash":

                    habilidad = new DashHab();
                    break;
                case "Invisibilidad":
                    habilidad = new InvisibleHab();
                    break;
                case "Explosiones":
                    habilidad = new ExplosionHab();
                    break;
                case "BombasDeHumo":
                    habilidad = new SmokeHab();
                    break;
            }
        }
    }
}
