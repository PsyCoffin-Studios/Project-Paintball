using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Cinemachine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


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
        if (IsClient&&IsOwner)
        {
            if (zoomOption)
            {
                if (zooming)
                {
                    thisWeapon.cinemachineCamera.Lens.FieldOfView = zoom;
                    UIReference.sprite = zoomCrosshair;
                    AjustarMira();
                }
                else
                {
                    thisWeapon.cinemachineCamera.Lens.FieldOfView = defaultZoom;
                    UIReference.sprite = crosshair;
                    AjustarMira();
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
    private void AjustarMira()
    {
       if(UIReference.sprite.rect.size.x > 32 && UIReference.sprite.rect.size.y > 32)
       {
            UIReference.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            UIReference.preserveAspect = false;
        }
        else
        {
            UIReference.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
            UIReference.preserveAspect = true;
        }
       
    }


    IEnumerator DisparoSeguido()
    {
        permisoDisparo = false;
        yield return new WaitForSeconds(0.2f);
        permisoDisparo = true;
        yield return null;
    }
}
