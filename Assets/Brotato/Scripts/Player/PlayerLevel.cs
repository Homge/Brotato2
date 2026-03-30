using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    [Header(" Settings ")]
    private int requiredXp;
    private int currentXp;
    private int level;
    private int levelsEarnedThisWave;

    [Header(" Visuals ")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header(" DEBUG ")]
    [SerializeField] private bool DEBUG;

    // ── Properties cho SaveManager ──
    public int CurrentLevel => level;
    public int CurrentXp    => currentXp;

    private void Awake()
    {
        Candy.onCollected += CandyCollectedCallback;
    }

    private void OnDestroy()
    {
        Candy.onCollected -= CandyCollectedCallback;
    }

    void Start()
    {
        UpdateRequiredXp();
        UpdateVisuals();
    }

    /// Gọi từ RunLoader khi load save
    public void LoadFromSave(int savedLevel, int savedXp)
    {
        level     = savedLevel;
        currentXp = savedXp;
        UpdateRequiredXp();
        UpdateVisuals();
    }

    private void UpdateRequiredXp() => requiredXp = (level + 1) * 5;

    private void UpdateVisuals()
    {
        xpBar.value    = (float)currentXp / requiredXp;
        levelText.text = "lvl " + (level + 1);
    }

    private void CandyCollectedCallback(Candy candy)
    {
        currentXp++;

        if (currentXp >= requiredXp)
            LevelUp();

        UpdateVisuals();
    }

    private void LevelUp()
    {
        level++;
        levelsEarnedThisWave++;
        currentXp = 0;
        UpdateRequiredXp();
    }

    public bool HasLeveledUp()
    {
        if (DEBUG)
            return true;

        if (levelsEarnedThisWave > 0)
        {
            levelsEarnedThisWave--;
            return true;
        }

        return false;
    }
}