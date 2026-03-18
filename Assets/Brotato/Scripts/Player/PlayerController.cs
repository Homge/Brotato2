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

    private float moveSpeed;

    // Chuyển việc lấy tham chiếu sang Awake để đảm bảo luôn chạy trước FixedUpdate
    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        moveSpeed = baseMoveSpeed;
    }   

    private void FixedUpdate()
    {   
        // Kiểm tra an toàn cho InputManager
        if (InputManager.instance == null) return;

        // Kiểm tra an toàn cho Rigidbody (đề phòng trường hợp component bị xóa nhầm)
        if (rig == null) return;

        Vector2 move = InputManager.instance.GetMoveVector();
        rig.linearVelocity = move * moveSpeed;
    }

    public void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        float moveSpeedPercent = playerStatsManager.GetStatValue(Stat.MoveSpeed) / 100;
        moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    }
}