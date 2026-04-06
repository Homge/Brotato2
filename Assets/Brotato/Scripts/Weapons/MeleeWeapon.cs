using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    enum State { Idle, Attack }
    private State state;

    [Header(" Elements ")]
    [SerializeField] private Transform hitDetectionTransform;
    [SerializeField] private BoxCollider2D hitCollider;

    [Header(" Animation Settings ")]
    [SerializeField] private float animationSpeedMultiplier = 1.2f;
    [SerializeField] private float damageWindowStart = 0.2f;
    [SerializeField] private float damageWindowEnd = 0.8f;

    [Header(" Settings ")]
    private List<Enemy> damagedEnemiesThisSwing = new List<Enemy>();
    private float currentAttackDuration;
    private bool isDamagePhase;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
        }
    }

    private void UpdateIdleState()
    {
        Enemy closestEnemy = GetClosestEnemy();
        Vector2 targetUpVector = Vector3.up;

        if (closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
            ManageAttack();
        }

        attackTimer += Time.deltaTime;
    }

    private void ManageAttack()
    {
        if (attackTimer >= attackDelay)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        state = State.Attack;
        attackTimer = 0;
        currentAttackDuration = 0;
        isDamagePhase = false;
        damagedEnemiesThisSwing.Clear();

        animator.Play("Attack");
        animator.speed = (1f / attackDelay) * animationSpeedMultiplier;

        PlayAttackSound();
    }

    private void UpdateAttackState()
    {
        currentAttackDuration += Time.deltaTime;
        
        // Tính toán phần trăm hoàn thành của chu kỳ đánh
        float normalizedTime = currentAttackDuration / (attackDelay / animationSpeedMultiplier);

        if (normalizedTime >= damageWindowStart && normalizedTime <= damageWindowEnd)
        {
            if (!isDamagePhase) isDamagePhase = true;
            PerformDamageCheck();
        }
        else if (isDamagePhase)
        {
            isDamagePhase = false;
        }

        // Kết thúc chu kỳ đánh
        if (currentAttackDuration >= attackDelay)
        {
            StopAttack();
        }
    }

  private void PerformDamageCheck()
{
  
    Vector2 boxSize = hitCollider.size;
    
    boxSize.x *= hitDetectionTransform.lossyScale.x;
    boxSize.y *= hitDetectionTransform.lossyScale.y;

    Collider2D[] enemies = Physics2D.OverlapBoxAll(
        hitDetectionTransform.position,
        boxSize, 
        hitDetectionTransform.eulerAngles.z, 
        enemyMask
    );

    for (int i = 0; i < enemies.Length; i++)
    {
        Enemy enemy = enemies[i].GetComponent<Enemy>();

        if (enemy != null && !damagedEnemiesThisSwing.Contains(enemy))
        {
            int damage = GetDamage(out bool isCriticalHit);
            enemy.TakeDamage(damage, isCriticalHit);
            damagedEnemiesThisSwing.Add(enemy);
        }
    }
}

    private void StopAttack()
    {
        state = State.Idle;
        damagedEnemiesThisSwing.Clear();
        isDamagePhase = false;
    }

    public override void UpdateStats(PlayerStatsManager playerStatsManager)
    {
        ConfigureStats();
        damage = Mathf.RoundToInt(damage * (1 + playerStatsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay /= 1 + (playerStatsManager.GetStatValue(Stat.AttackSpeed) / 100);
        criticalChance = Mathf.RoundToInt(criticalChance * (1 + playerStatsManager.GetStatValue(Stat.CriticalChance) / 100));
        criticalPercent += playerStatsManager.GetStatValue(Stat.CriticalPercent);    
    }
}