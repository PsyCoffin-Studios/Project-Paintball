using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlayerController body;
    
    void Start()
    {
        //if(isOwner){
        GetComponent<PlayerInput>().enabled = true;
        //}

        body = transform.GetChild(0).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        body.hor = context.ReadValue<Vector2>().x;
        body.ver = context.ReadValue<Vector2>().y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            body.Saltar();
        }
    }

    public void OnMouseX(InputAction.CallbackContext context){
        body.horM = context.ReadValue<float>();
    }
    public void OnMouseY(InputAction.CallbackContext context){
        body.verM = context.ReadValue<float>();
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
}
