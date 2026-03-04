using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IPlayerStatsDependency
{
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

    [Header(" Settings ")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask enemyMask;

    [Header(" Attack ")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected Animator animator;

    protected float attackTimer;

    [Header(" Critical ")]
    protected int criticalChance;
    protected float criticalPercent;

    [Header(" Animations ")]
    [SerializeField] protected float aimLerp;

    [Header(" Level ")]
    public int Level { get; private set; }


    [Header(" Audio ")]
    protected AudioSource audioSource;

    protected void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = WeaponData.AttackSound;

        if(animator != null && WeaponData.AnimatorOverride != null)
            animator.runtimeAnimatorController = WeaponData.AnimatorOverride;
    }

    protected void PlayAttackSound()
    {
        if (!AudioManager.instance.IsSFXOn)
            return;

        audioSource.pitch = Random.Range(.95f, 1.05f);
        audioSource.Play();
    }

    protected Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);

        if (enemies.Length <= 0)
            return null;        

        float minDistance = range;

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyChecked = enemies[i].GetComponent<Enemy>();

            float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);

            if (distanceToEnemy < minDistance)
            {
                closestEnemy = enemyChecked;
                minDistance = distanceToEnemy;
            }
        }

        return closestEnemy;
    }

    protected int GetDamage(out bool isCriticalHit)
    {
        isCriticalHit = false;

        if (Random.Range(0, 101) <= criticalChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(damage * criticalPercent);
        }

        return damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public abstract void UpdateStats(PlayerStatsManager playerStatsManager);

    protected void ConfigureStats()
    {
        Dictionary<Stat, float> calculatedStats = WeaponStatsCalculator.GetStats(WeaponData, Level);

        damage              =   Mathf.RoundToInt(calculatedStats[Stat.Attack]);
        attackDelay         =   1f / calculatedStats[Stat.AttackSpeed];
        criticalChance      =   Mathf.RoundToInt(calculatedStats[Stat.CriticalChance]);
        criticalPercent     =   calculatedStats[Stat.CriticalPercent];
        range               =   calculatedStats[Stat.Range];
    }

    public void UpgradeTo(int targetLevel)
    {
        Level = targetLevel;
        ConfigureStats();
    }

    public int GetRecyclePrice()
    {
        return WeaponStatsCalculator.GetRecyclePrice(WeaponData, Level);
    }

    public void Upgrade() => UpgradeTo(Level + 1);
}