using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayerStatsDependency
{
    [Header("Elements")]
    private Rigidbody2D rig;

    [Header(" Settings ")]
    [SerializeField] private float baseMoveSpeed;

    private Vector2 Key;

    private float moveSpeed;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        moveSpeed = baseMoveSpeed;
    }

    private void FixedUpdate()
    {
        rig.linearVelocity = InputManager.instance.GetMoveVector() * moveSpeed * Time.fixedDeltaTime;
    }
    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        float moveSpeedPercent = playerStatsManager.GetStatValue(Stat.MoveSpeed) / 100;
        moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    }

}