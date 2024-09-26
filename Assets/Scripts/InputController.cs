using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Cinemachine;

public class InputController : NetworkBehaviour
{
    [SerializeField] private PlayerController body;
    

    void Start()
    {
        //body = transform.GetChild(0).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
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
        }
        else if (context.canceled)
        {
            body.ExitCrouch();
        }
    }


    [ServerRpc]
    public void OnMoveServerRpc(Vector2 input)
    {
        body.hor = input.x;
        body.ver = input.y;
    }

    [ServerRpc]
    public void OnJumpServerRpc()
    {

    }


    [ServerRpc]
    public void OnCrouchServerRpc()
    {

    }
}
