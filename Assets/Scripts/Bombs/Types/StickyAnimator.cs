using UnityEngine;

public class StickyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator = default;

    private void OnEnable()
    {
        //animator.SetBool("Bool", true);
        animator.Play("New State");
    }

    private void OnDisable()
    {
        //animator.SetBool("Bool", false);
    }

    private void LateUpdate()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
        {
            ObjectPooler.singleton.Despawn(gameObject);
        }
    }
}
