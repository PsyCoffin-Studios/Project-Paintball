using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine.Windows;
using Unity.Burst.Intrinsics;

public class InputController : NetworkBehaviour
{
    [SerializeField] protected PlayerController body;
    [SerializeField] protected WeaponController gun;


    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;


    void Awake()
    {
        inputActions = GetComponent<PlayerInput>().actions;
        playerActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");

        EnablePlayerControls();
    }

    public void EnablePlayerControls()
    {
        uiActionMap.Disable();
        playerActionMap.Enable();
    }

    public void EnableUIControls()
    {
        playerActionMap.Disable();
        uiActionMap.Enable();
    }

    public void SetGun(WeaponController g)
    {
        gun = g;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveServerRpc(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            body.Saltar();
        }
    }

    

    public void OnMouseX(InputAction.CallbackContext context){
        OnMouseXServerRpc(context.ReadValue<float>());
    }
    public void OnMouseY(InputAction.CallbackContext context){
        
        OnMouseYServerRpc(context.ReadValue<float>());
    }

    [ServerRpc]
    public void OnMouseXServerRpc(float input)
    {
        //Debug.Log(input);
        body.horM = input;
    }

    [ServerRpc]
    public void OnMouseYServerRpc(float input)
    {
        body.verM = input;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            body.EnterCrouch();
            gun.ChangeReduction(gun.crouchReduction);
        }
        else if (context.canceled)
        {
            body.ExitCrouch();
            gun.ChangeReduction(gun.defaultReduction);
        }
    }


    [ServerRpc]
    public void OnMoveServerRpc(Vector2 input)
    {
        body.hor = input.x;
        body.ver = input.y;
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (gun.holdOption)
        {
            if (context.performed)
            {
                gun.holding = true;
            }
            if (context.canceled){
                gun.holding = false;
            }
        }
        else {
            if (context.performed)
            {
                gun.ShootBullet();
            }
        }
        
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (gun.zoomOption)
        {
            if (context.performed)
            {
                gun.ChangeReduction(gun.zoomReduction);
                gun.zooming = true;
            }
            else if (context.canceled)
            {
                gun.ChangeReduction(gun.defaultReduction);
                gun.zooming = false;
            }
        }

    }

    public void OnPause(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            GameObject.Find("@UIManager").GetComponent<UIControllerInGame>().AbrirOpciones(gameObject);
        }
    }

    public void OnQuitPause(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            GameObject.Find("@UIManager").GetComponent<UIControllerInGame>().CerrarOpciones();
        }
    }


    public void OnHabilidad(InputAction.CallbackContext context)
    {
        body.UsarHabilidad();
    }


}
