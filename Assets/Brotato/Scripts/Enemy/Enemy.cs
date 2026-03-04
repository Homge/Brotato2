using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour
{
    [Header(" Components ")]
    protected EnemyMovement movement;

    [Header(" Health ")]
    [SerializeField] protected int maxHealth;
    protected int health;

    [Header(" Elements ")]
    protected Player player;

    [Header(" Spawn Sequence Related ")]
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] protected Collider2D collider;
    protected bool hasSpawned;

    [Header(" Effects ")]
    [SerializeField] protected ParticleSystem passAwayParticles;

    [Header(" Attack ")]
    [SerializeField] protected float playerDetectionRadius;

    // Dmg - position - isCritical
    [Header(" Actions ")]
    public static Action<int, Vector2, bool> onDamageTaken;
    // position
    public static Action<Vector2> onPassedAway;
    public static Action<Vector2> onBossPassedAway;

    protected Action onSpawnSequenceCompleted;

    [Header(" DEBUG ")]
    [SerializeField] protected bool gizmos;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        movement = GetComponent<EnemyMovement>();
        player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogWarning("No player found, Auto-destroying...");
            Destroy(gameObject);
        }

        StartSpawnSequence();
    }

    // Update is called once per frame
    protected bool CanAttack()
    {
        return renderer.enabled;
    }

    private void StartSpawnSequence()
    {
        SetRenderersVisibility(false);

        // Scale up & down the spawn indicator
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceCompleted);
    }

    private void SpawnSequenceCompleted()
    {
        SetRenderersVisibility();
        hasSpawned = true;

        collider.enabled = true;

        if (movement != null)
            movement.StorePlayer(player);

        onSpawnSequenceCompleted?.Invoke();    
    }

    private void SetRenderersVisibility(bool visibility = true)
    {
        renderer.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }

    public void TakeDamage(int damage, bool isCriticalHit)
    {
        int realDamage = Mathf.Min(damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(damage, transform.position, isCriticalHit);

        if (health <= 0)
            PassAway();
    }

    public virtual void PassAway()
    {
        onPassedAway?.Invoke(transform.position);
        PassAwayAfterWave();
    }

    public void PassAwayAfterWave()
    {
        passAwayParticles.transform.SetParent(null);
        passAwayParticles.Play();

        Destroy(gameObject);
    }

    public Vector2 GetCenter()
    {
        return (Vector2)transform.position + collider.offset;
    }

    private void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
