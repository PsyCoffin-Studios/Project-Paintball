using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputController : InputController
{

    bool crouch = false;
    bool zoom = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseXY(Vector2 a)
    {
        OnMouseXServerRpc(a.x);
        OnMouseYServerRpc(a.y);
    }

    //variaciones para movil (deberia hacerlas en otro script pero son las 2 de la mañana)

    public void OnHabilidadMobile()
    {
        body.UsarHabilidad();
    }

    public void OnCrouchMobile()
    {
        crouch = !crouch;
        if (crouch)
        {
            body.EnterCrouch();
            gun.ChangeReduction(gun.crouchReduction);
        }
        else
        {
            body.ExitCrouch();
            gun.ChangeReduction(gun.defaultReduction);
        }
    }

    public void OnZoomMobile()
    {
        if (gun.zoomOption)
        {
            zoom = !zoom;
            if (zoom)
            {
                gun.ChangeReduction(gun.zoomReduction);
                gun.zooming = true;
            }
            else
            {
                gun.ChangeReduction(gun.defaultReduction);
                gun.zooming = false;
            }
        }

    }


    public void OnShootMobile(bool a)
    {
        if (gun.holdOption)
        {
            if (a)
            {
                gun.holding = true;
            }
            else
            {
                gun.holding = false;
            }
        }
        else
        {
            if (a)
            {
                gun.ShootBullet();
            }
        }

    }

    public void OnJumpMobile()
    {
        body.Saltar();
    }

    public void OnPauseMobile()
    {
        GameObject.Find("@UIManager").GetComponent<UIControllerInGame>().AbrirOpciones(gameObject);
    }
}
