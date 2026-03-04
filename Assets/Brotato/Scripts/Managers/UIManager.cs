using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header(" Panels ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectionPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject restartConfirmationPanel;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject settingsPanel;

    private List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        panels.AddRange(new GameObject[]
        {
            menuPanel,
            weaponSelectionPanel,
            gamePanel,
            gameoverPanel,
            stageCompletePanel,
            waveTransitionPanel,
            shopPanel
        });



        GameManager.onGamePaused    += GamePausedCallback;
        GameManager.onGameResumed   += GameResumedCallback;

        pausePanel.SetActive(false);
        HideRestartConfirmationPanel();

        HideCharacterSelection();

        HideSettings();
    }

    
    private void OnDestroy()
    {
        GameManager.onGamePaused    -= GamePausedCallback;
        GameManager.onGameResumed   -= GameResumedCallback; 
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.MENU:
                ShowPanel(menuPanel);
                break;

            case GameState.WEAPONSELECTION:
                ShowPanel(weaponSelectionPanel);
                break;

            case GameState.GAME:
                ShowPanel(gamePanel);
                break;

            case GameState.GAMEOVER:
                ShowPanel(gameoverPanel);
                break;

            case GameState.STAGECOMPLETE:
                ShowPanel(stageCompletePanel);
                break;

            case GameState.WAVETRANSITION:
                ShowPanel(waveTransitionPanel);
                break;

            case GameState.SHOP:
                ShowPanel(shopPanel);
                break;
        }
    }

    private void ShowPanel(GameObject panel)
    {
        foreach (GameObject p in panels)
            p.SetActive(p == panel);
    }


    private void GamePausedCallback() => pausePanel.SetActive(true);
    private void GameResumedCallback() => pausePanel.SetActive(false);

    public void ShowRestartConfirmationPanel() => restartConfirmationPanel.SetActive(true);    
    public void HideRestartConfirmationPanel() => restartConfirmationPanel.SetActive(false);

    public void ShowCharacterSelection() => characterSelectionPanel.SetActive(true);
    public void HideCharacterSelection() => characterSelectionPanel.SetActive(false);

    public void ShowSettings() => settingsPanel.SetActive(true);
    public void HideSettings() => settingsPanel.SetActive(false);
}
