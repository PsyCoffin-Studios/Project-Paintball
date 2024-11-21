using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animation1, animation2;
    
    void OnEnable()
    {
        animator.Play(animation1);
    }

    public void CambioAnimacion()
    {
        float numeroAleatorio = Random.Range(0f, 1f);
        Debug.Log("Número generado: " + numeroAleatorio);

        if(numeroAleatorio < 0.3f)
        {
            animator.Play(animation2);
        }
    }
}
