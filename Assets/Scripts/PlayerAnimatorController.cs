using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetState(int _animState)
    {
        animator.SetInteger("state", _animState);
    }
}
