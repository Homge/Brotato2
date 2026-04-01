using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header(" Elements ")]
    private Rigidbody2D rig;
    private Collider2D collider;
    private RangeWeapon rangeWeapon;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask enemyMask;
    
    [Header(" Pierce Settings ")]
    [Tooltip("Số lượng quái có thể bắn xuyên. Đặt = 1 là súng thường, = 3 là xuyên 3 con, = -1 là xuyên vô hạn")]
    [SerializeField] private int pierceCount = 1; 
    private int currentPierceCount;
    private List<Enemy> hitEnemies = new List<Enemy>();

    private int damage;
    private bool isCriticalHit;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    public void Configure(RangeWeapon rangeWeapon)
    {
        this.rangeWeapon = rangeWeapon;
    }

    // Nhận thêm biến extraPierce từ súng truyền vào
    public void Shoot(int damage, Vector2 direction, bool isCriticalHit, float range, int extraPierce = 0)
    {
        float lifeTime = (moveSpeed > 0) ? (range / moveSpeed) : 1f;
        Invoke("Release", lifeTime);

        this.damage = damage;
        this.isCriticalHit = isCriticalHit;
        
      
        if (pierceCount == -1) 
        {
            currentPierceCount = -1; 
        }
        else 
        {
            currentPierceCount = pierceCount + extraPierce; // Súng thường + Item cộng thêm số pierce, súng xuyên + Item cộng thêm số pierce
        }

        hitEnemies.Clear();

        transform.right = direction;
        rig.linearVelocity = direction * moveSpeed;
    }

    public void Reload()
    {
        hitEnemies.Clear();
        rig.linearVelocity = Vector2.zero;
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                Attack(enemy);

                if (currentPierceCount > 0) 
                {
                    currentPierceCount--;
                    if (currentPierceCount <= 0)
                    {
                        CancelInvoke();
                        Release();      
                    }
                }
            }
        }
    }

    private void Release()
    {
        if (!gameObject.activeSelf)
            return;
        rangeWeapon.ReleaseBullet(this);
    }

    private void Attack(Enemy enemy)
    {
        enemy.TakeDamage(damage, isCriticalHit);
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}