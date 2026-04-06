using UnityEngine;

public class TankerEnemy : MeleeEnemy
{
    [Header(" Tanker Stats ")]
    [SerializeField] private int damageReduction = 5;
    [SerializeField] private float sizeMultiplier = 1.5f;

    protected override void Start()
    {
        base.Start();
        
        // Tỷ lệ máu cơ bản cao gấp 3 lần so với MeleeEnemy thông thường
        maxHealth = Mathf.RoundToInt(maxHealth * 3f);
        health = maxHealth;

        // Tăng kích thước tổng thể
        transform.localScale *= sizeMultiplier;
    }

    public override void TakeDamage(int damage, bool isCriticalHit)
    {
        // Công thức giáp chặn: Sát thương thực tế = Sát thương gốc - Giáp. 
        // Đảm bảo sát thương tối thiểu là 1 để tránh bất tử.
        int reducedDamage = Mathf.Max(1, damage - damageReduction);
        
        // Kích hoạt hiệu ứng nhấp nháy áo giáp (phản hồi thị giác)
        LeanTween.cancel(gameObject);
        LeanTween.color(gameObject, Color.gray, 0.1f).setLoopPingPong(1);

        base.TakeDamage(reducedDamage, isCriticalHit);
    }
}