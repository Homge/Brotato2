using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using Random = UnityEngine.Random;

public class DropManager : MonoBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private Candy candyPrefab;
    [SerializeField] private Cash cashPrefab;
    [SerializeField] private Chest chestPrefab;


    [Header(" Settings ")]
    [SerializeField] [Range(0, 100)] private int cashDropChance;
    [SerializeField] [Range(0, 100)] private int chestDropChance;

    [Header(" Pooling ")]
    private ObjectPool<Candy> candyPool;
    private ObjectPool<Cash> cashPool;

    private void Awake()
    {
        Enemy.onPassedAway      += EnemyPassedAwayCallback;
        Enemy.onBossPassedAway  += BossEnemyPassedAwayCallback;
        Candy.onCollected       += ReleaseCandy;
        Cash.onCollected        += ReleaseCash;
    }

    private void OnDestroy()
    {
        Enemy.onPassedAway      -= EnemyPassedAwayCallback;
        Enemy.onBossPassedAway  -= BossEnemyPassedAwayCallback;
        Candy.onCollected       -= ReleaseCandy;
        Cash.onCollected        -= ReleaseCash;
    }

    // Start is called before the first frame update
    void Start()
    {
        candyPool = new ObjectPool<Candy>(
            CandyCreateFunction, 
            CandyActionOnGet, 
            CandyActionOnRelease, 
            CandyActionOnDestroy);

        cashPool = new ObjectPool<Cash>(
            CashCreateFunction, 
            CashActionOnGet, 
            CashActionOnRelease, 
            CashActionOnDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Candy CandyCreateFunction()             => Instantiate(candyPrefab, transform);
    private void CandyActionOnGet(Candy candy)      => candy.gameObject.SetActive(true);
    private void CandyActionOnRelease(Candy candy)  => candy.gameObject.SetActive(false);
    private void CandyActionOnDestroy(Candy candy)  => Destroy(candy.gameObject);

    private Cash CashCreateFunction()               => Instantiate(cashPrefab, transform);
    private void CashActionOnGet(Cash cash)         => cash.gameObject.SetActive(true);
    private void CashActionOnRelease(Cash cash)     => cash.gameObject.SetActive(false);
    private void CashActionOnDestroy(Cash cash)     => Destroy(cash.gameObject);



    private void BossEnemyPassedAwayCallback(Vector2 BossPosition)
    {
       DropChest(BossPosition);
    }

   private void EnemyPassedAwayCallback(Vector2 enemyPosition)
    {
        int scaledCashChance = Mathf.Min(100, Mathf.RoundToInt(cashDropChance * Mathf.Sqrt(WaveManager.DifficultyMultiplier)));
        bool shouldSpawnCash = Random.Range(0, 101) <= scaledCashChance;

        DroppableCurrency dropppable = shouldSpawnCash ? cashPool.Get() : candyPool.Get();
        dropppable.transform.position = enemyPosition;

        TryDropChest(enemyPosition);
    }

    private void TryDropChest(Vector2 spawnPosition)
    {
        bool shouldSpawnChest = Random.Range(0, 101) <= chestDropChance;

        if (!shouldSpawnChest)
            return;

        DropChest(spawnPosition);
    }
    private void DropChest(Vector2 spawnPosition)
    {
        Instantiate(chestPrefab, spawnPosition, Quaternion.identity, transform);
    }

    private void ReleaseCandy(Candy candy)  => candyPool.Release(candy);
    private void ReleaseCash(Cash cash)     => cashPool.Release(cash);
    public void GameStateChangedCallback(GameState gameState)
    {
        // Khi kết thúc wave (chuyển sang chuyển tiếp, cửa hàng, hoặc thua cuộc)
        if (gameState == GameState.SHOP || gameState == GameState.WAVETRANSITION || gameState == GameState.GAMEOVER)
        {
            CleanUpDrops();
        }
    }

    private void CleanUpDrops()
    {
       
        Candy[] activeCandies = transform.GetComponentsInChildren<Candy>(false);
        foreach (Candy candy in activeCandies)
        {
            candyPool.Release(candy);
        }

        Cash[] activeCash = transform.GetComponentsInChildren<Cash>(false);
        foreach (Cash cash in activeCash)
        {
            cashPool.Release(cash);
        }

    
        Chest[] activeChests = transform.GetComponentsInChildren<Chest>(false);
        foreach (Chest chest in activeChests)
        {
            Destroy(chest.gameObject);
        }
    }
}
