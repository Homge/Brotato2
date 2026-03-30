using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header(" Actions ")]
    public static Action onGamePaused;
    public static Action onGameResumed;

    [Header(" Menu UI ")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject warningPanel;

    [Header(" References ")]
    [SerializeField] private RunLoader runLoader;
    [SerializeField] private Player player;

    public GameState CurrentState { get; private set; }

    private bool isPausedFromShop = false;

   private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        SetGameState(GameState.MENU);
    }

    public void StartGame()            => SetGameState(GameState.GAME);
    public void StartWeaponSelection() => SetGameState(GameState.WEAPONSELECTION);
    public void StartShop()            => SetGameState(GameState.SHOP);

    public void SetGameState(GameState gameState)
    {
        CurrentState = gameState;

        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener listener in gameStateListeners)
            listener.GameStateChangedCallback(gameState);

        if (gameState == GameState.MENU)
            RefreshMenuUI();
    }

    // ── WAVE ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Gọi bởi WaveManager sau khi wave kết thúc,
    /// và bởi WaveTransitionManager sau khi chọn xong upgrade.
    /// Nếu player còn level chưa dùng → WAVETRANSITION, không còn → SHOP.
    /// </summary>
    public void WaveCompletedCallback()
    {
        if (player != null && player.HasLeveledUp())
            SetGameState(GameState.WAVETRANSITION);
        else
            StartShop();
    }

    // ── MENU ──────────────────────────────────────────────────────────────────

    private void RefreshMenuUI()
    {
        bool hasSave = SaveManager.instance != null && SaveManager.instance.HasSave();

        if (continueButton != null)
            continueButton.SetActive(hasSave);

        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    public void OnNewGameClick()
    {
        bool hasSave = SaveManager.instance != null && SaveManager.instance.HasSave();

        if (hasSave && warningPanel != null)
            warningPanel.SetActive(true);
        else
            ConfirmNewGame();
    }

    public void ConfirmNewGame()
    {
        if (warningPanel != null) warningPanel.SetActive(false);
        SaveManager.instance?.DeleteSave();
        StartWeaponSelection();
    }

    public void CancelNewGame()
    {
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    // ── CONTINUE / SAVE ───────────────────────────────────────────────────────

    public void ContinueGame()
    {
        if (SaveManager.instance == null || !SaveManager.instance.HasSave())
            return;

        // Đọc save TRƯỚC khi load để biết nên về GAME hay SHOP
        RunSaveData data = SaveManager.instance.LoadRun();

        // Nạp dữ liệu player/wave
        if (runLoader != null)
            runLoader.LoadAndContinue();

        // BUG FIX: Restore đúng trạng thái thay vì luôn về SHOP
        if (data != null && data.savedGameState == GameState.GAME.ToString())
        {
            Debug.Log("[GameManager] Continue → GAME (mid-wave save)");
            SetGameState(GameState.GAME);
        }
        else
        {
            Debug.Log("[GameManager] Continue → SHOP");
            SetGameState(GameState.SHOP);
        }
    }

    // ── PAUSE ─────────────────────────────────────────────────────────────────

    public void PauseGame()
    {
        isPausedFromShop = (CurrentState == GameState.SHOP);

        if (!isPausedFromShop)
            Time.timeScale = 0f;

        onGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!isPausedFromShop)
            Time.timeScale = 1f;

        isPausedFromShop = false;
        onGameResumed?.Invoke();
    }

    public void SaveAndReturnToMenu()
    {
        Time.timeScale = 1f;
        isPausedFromShop = false;

        SaveManager.instance?.SaveRun();

        onGameResumed?.Invoke();
        SceneManager.LoadScene(0);
    }

    public void AbandonAndReturnToMenu()
    {
        Time.timeScale = 1f;
        isPausedFromShop = false;

        SaveManager.instance?.DeleteSave();
        onGameResumed?.Invoke();
        SceneManager.LoadScene(0);
    }

    // ── GAMEOVER / STAGE COMPLETE ─────────────────────────────────────────────

     public void ManageGameover()
    {
        Time.timeScale = 1f;
        SaveManager.instance?.DeleteSave();
        SceneManager.LoadScene(0);
    }

    public void ContinueToEndlessMode()
    {
        if (WaveManager.instance != null)
            WaveManager.instance.EnableEndlessMode();

        SetGameState(GameState.GAME);
    }
}