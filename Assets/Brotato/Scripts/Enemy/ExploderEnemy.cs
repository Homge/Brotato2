using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class ExploderEnemy : Enemy
{
    [Header(" Explosion ")]
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDelay = 0.8f;

    private bool isExploding = false;

    protected override void Start()
    {
        base.Start();
        explosionDamage = Mathf.RoundToInt(explosionDamage * WaveManager.DifficultyMultiplier);
    }

    void Update()
    {
        if (!CanAttack() || isExploding) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= playerDetectionRadius)
        {
            StartExplosion();
        }
        else
        {
            movement.FollowPlayer();
        }
    }

    private void StartExplosion()
    {
        isExploding = true;

        LeanTween.color(renderer.gameObject, Color.red, explosionDelay).setEasePunch();
        LeanTween.scale(gameObject, transform.localScale * 1.3f, explosionDelay);
        
        Invoke(nameof(Explode), explosionDelay);
    }

   private void Explode()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= explosionRadius)
        {
            player.TakeDamage(explosionDamage);
        }

        // Thay vì gọi PassAwayAfterWave(), gọi base.PassAway() để kế thừa logic rớt kẹo, kích hoạt hạt nổ và tự động xóa Object.
        base.PassAway(); 
    }

    public override void PassAway()
    {
        // Xử lý trường hợp bị người chơi tiêu diệt trước khi thời gian đếm ngược kết thúc
        if (!isExploding) 
        {
            CancelInvoke(nameof(Explode));
            
            // Ép nổ gây sát thương ngay lập tức nếu người chơi đứng trong phạm vi
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= explosionRadius)
            {
                player.TakeDamage(explosionDamage);
            }
        }
        
        // Kích hoạt hạt nổ và xóa Object (Áp dụng cho cả khi bị bắn chết)
        base.PassAway(); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}