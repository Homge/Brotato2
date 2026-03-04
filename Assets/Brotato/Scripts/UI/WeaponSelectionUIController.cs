using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionUIController : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Button goButton;
    [SerializeField] private Text goButtonText;

    [Header(" Settings ")]
    [SerializeField] private Color disabledButtonColor = Color.gray;
    [SerializeField] private Color enabledButtonColor = Color.white;

    private WeaponSelectionManager weaponSelectionManager;
    private Button goButtonComponent;

    private void Awake()
    {
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();

        if (goButton == null)
            goButton = GetComponent<Button>();

        goButtonComponent = goButton;
        
        // Set initial state
        UpdateButtonState();
    }

    private void Start()
    {
        // Update button state when weapon is selected
        // We'll check this in Update for simplicity
    }

    private void Update()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (weaponSelectionManager == null)
            return;

        bool isWeaponSelected = weaponSelectionManager.IsWeaponSelected();

        goButtonComponent.interactable = isWeaponSelected;

        // Optional: Change button color to indicate enabled/disabled state
        if (goButtonText != null)
        {
            goButtonText.color = isWeaponSelected ? enabledButtonColor : disabledButtonColor;
        }
    }
}
