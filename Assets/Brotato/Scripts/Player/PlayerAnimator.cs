using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    private Rigidbody2D rig;

    private void Awake() => rig = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (rig.linearVelocity.magnitude < 0.001)
            animator.Play("Idle");
        else
            animator.Play("Move");
    }
}
