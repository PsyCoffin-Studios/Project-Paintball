using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Cinemachine;


public class WeaponController : NetworkBehaviour
{
    Weapon thisWeapon;
    public bool holdOption;
    public bool zoomOption;
    [SerializeField] float zoom; //40 para el rifle y 10 para el sniper
    [SerializeField] float defaultZoom = 60;

    public bool holding;
    public bool zooming;

    public float crouchReduction; //reducción del desvio por agacharse
    public float zoomReduction; //reduccion del desvio por apuntar
    public float defaultReduction = 1f;

    public Image UIReference;
    public Sprite crosshair;
    public Sprite zoomCrosshair;


    public bool permisoDisparo = true;
    public Transform _camTransform;
   
    public AudioSource _audioSource;

    void Start()
    {

        thisWeapon = GetComponent<Weapon>();
        thisWeapon.reduccionDeDesvio = defaultReduction;

        thisWeapon.cinemachineCamera = _camTransform.GetComponent<CinemachineCamera>();


        if (IsOwner)
        {
            UIReference = GameObject.Find("CROSSHAIR").GetComponent<Image>();
            UIReference.sprite = crosshair;

            _audioSource = this.GetComponentInChildren<AudioSource>();

            thisWeapon.cinemachineCamera.Priority = 1; //Asignamos la prioridad de la cámara para que solo la coja el propietario
        }
        else
        {
            thisWeapon.cinemachineCamera.Priority = 0;
            if (thisWeapon.cinemachineCamera == null)
            {
                Debug.LogError("No se encontró la CinemachineCamera para este cliente (ARMA) .");
                return; // Sale si no encuentra la cámara
            }
        }

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (zoomOption && IsOwner)
        {
            if (zooming)
            {
                thisWeapon.cinemachineCamera.Lens.FieldOfView = zoom;
                UIReference.sprite = zoomCrosshair;
                AdjustRectTransformToSprite(zoomCrosshair);
            }
            else
            {
                thisWeapon.cinemachineCamera.Lens.FieldOfView = defaultZoom;
                UIReference.sprite = crosshair;
                AdjustRectTransformToSprite(crosshair);
            }
        }
        if (holdOption)
        {
            if (holding)
            {
                if (permisoDisparo)
                {
                    StartCoroutine(DisparoSeguido());
                    ShootBullet();
                }
            }
        }
    }


    public void ShootBullet()
    {
        thisWeapon.ShootBullet();
        _audioSource.Play();
        if (zoomOption == true && holdOption == false)
        {
            if (zooming) { zooming = false; }
        }
    }

    public void ChangeReduction(float a)
    {
        thisWeapon.reduccionDeDesvio = a;
    }


    private void AdjustRectTransformToSprite(Sprite sprite)
    {
       RectTransform rt = UIReference.GetComponent<RectTransform>();
       rt.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);

        /*
        RectTransform rt = UIReference.GetComponent<RectTransform>();

        // Obtener el tamaño de la pantalla
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Ajustar el tamaño del RectTransform para que coincida con el tamaño de la pantalla
        rt.sizeDelta = new Vector2(screenWidth, screenHeight);
        */
    }


    IEnumerator DisparoSeguido()
    {
        permisoDisparo = false;
        yield return new WaitForSeconds(0.2f);
        permisoDisparo = true;
        yield return null;
    }
}
