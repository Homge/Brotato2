using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionManager : MonoBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private WeaponSelectionContainer weaponContainerPrefab;
    [SerializeField] private PlayerWeapons playerWeapons;
    [SerializeField] private Button goButton;

    [Header(" Data ")]
    [SerializeField] private WeaponDataSO[] starterWeapons;
    private WeaponDataSO selectedWeapon;
    private int initialWeaponLevel;
    private bool isInWeaponSelection = false;

    public void GameStateChangedCallback(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GAME:

                if (selectedWeapon == null)
                {
                    // Stay in weapon selection screen
                    GameManager.instance.SetGameState(GameState.WEAPONSELECTION);
                    return;
                }

                isInWeaponSelection = false;
                playerWeapons.TryAddWeapon(selectedWeapon, initialWeaponLevel);
                selectedWeapon = null;
                initialWeaponLevel = 0;

                break;

            case GameState.WEAPONSELECTION:
                isInWeaponSelection = true;
                Configure();
                break;

            default:
                isInWeaponSelection = false;
                break;
        }
    }

    [NaughtyAttributes.Button]
    private void Configure()
    {
        // Clean our parent, no children
        containersParent.Clear();

        // Disable Go button until weapon is selected
        if (goButton != null)
            goButton.interactable = false;

        selectedWeapon = null;
        initialWeaponLevel = 0;

        // Generate weapon containers
        for (int i = 0; i < 3; i++)
            GenerateWeaponContainer();
    }

    private void GenerateWeaponContainer()
    {
        WeaponSelectionContainer containerInstance = Instantiate(weaponContainerPrefab, containersParent);

        WeaponDataSO weaponData = starterWeapons[Random.Range(0, starterWeapons.Length)];

        int level = Random.Range(0, 4);

        containerInstance.Configure(weaponData, level);

        containerInstance.Button.onClick.RemoveAllListeners();
        containerInstance.Button.onClick.AddListener(() => WeaponSelectedCallback(containerInstance, weaponData, level));
    }

    private void WeaponSelectedCallback(WeaponSelectionContainer containerInstance, WeaponDataSO weaponData, int level)
    {
        selectedWeapon = weaponData;
        initialWeaponLevel = level;

        // Enable Go button only when in weapon selection
        if (isInWeaponSelection && goButton != null)
            goButton.interactable = true;

        foreach (WeaponSelectionContainer container in containersParent.GetComponentsInChildren<WeaponSelectionContainer>())
        {
            if (container == containerInstance)
                container.Select();
            else
                container.Deselect();
        }
    }

    public bool IsWeaponSelected()
    {
        return selectedWeapon != null;
    }
}
