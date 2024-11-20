using UnityEngine;

using Unity.Netcode;

public class DestroyOnAnimationEnd : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 1.3f;
        
    }

    void Update()
    {
        if (!gameObject.transform.root.GetComponent<NetworkObject>().IsSpawned) {  return; }
        // Verifica si la animación ha terminado
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            !animator.IsInTransition(0))
        {

            gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }

}
