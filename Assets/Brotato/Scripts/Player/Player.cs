using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth), typeof(PlayerLevel))]
public class Player : MonoBehaviour, IGameStateListener
{
    public static Player instance;

    [Header(" Components ")]
    [SerializeField] private CircleCollider2D collider;
    [SerializeField] private SpriteRenderer playerRenderer;
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;

    private void Awake()
    {
        instance = this;

        playerHealth = GetComponent<PlayerHealth>();
        playerLevel = GetComponent<PlayerLevel>();

        CharacterSelectionManager.onCharacterSelected += CharacterSelectedCallback;
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.onCharacterSelected -= CharacterSelectedCallback;
    }

    private void CharacterSelectedCallback(CharacterDataSO characterData)
    {
        playerRenderer.sprite = characterData.Sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.GAME)
            transform.position = Vector3.zero;
    }

    public void TakeDamage(int damage)
    {
        playerHealth.TakeDamage(damage);
    }

    public Vector2 GetCenter()
    {
        return (Vector2)transform.position + collider.offset;
    }

    public bool HasLeveledUp()
    {
        return playerLevel.HasLeveledUp();
    }
}