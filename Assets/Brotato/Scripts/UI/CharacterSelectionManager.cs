using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tabsil.Sijil;
using System;

public class CharacterSelectionManager : MonoBehaviour, IWantToBeSaved
{
    [Header(" Elements ")]
    [SerializeField] private Transform characterButtonsParent;
    [SerializeField] private CharacterButton characterButtonPrefab;
    [SerializeField] private Image centerCharacterImage;
    [SerializeField] private CharacterInfoPanel characterInfo;

    [Header(" Data ")]
    private CharacterDataSO[] characterDatas;
    private List<bool> unlockedStates = new List<bool>();
    private const string unlockedStatesKey = "unlockedStatesKey";
    private const string lastSelectedCharacterKey = "lastSelectedCharacterKey";

    [Header(" Settings ")]
    private int lastSelectedCharacterIndex;
    private int selectedCharacterIndex;


    [Header(" Actions ")]
    public static Action<CharacterDataSO> onCharacterSelected;

    private void Awake()
    {
              
    }

    // Start is called before the first frame update
    void Start()
    {
        characterInfo.Button.onClick.RemoveAllListeners();
        characterInfo.Button.onClick.AddListener(PurchaseSelectedCharacter);

        CharacterSelectedCallback(lastSelectedCharacterIndex);
    }


    private void Initialize()
    {
        for (int i = 0; i < characterDatas.Length; i++)
            CreateCharacterButton(i);        
    }

    private void CreateCharacterButton(int index)
    {
        CharacterDataSO characterData = characterDatas[index];

        CharacterButton characterButtonInstance = Instantiate(characterButtonPrefab, characterButtonsParent);
        characterButtonInstance.Configure(characterData.Sprite, unlockedStates[index]);

        characterButtonInstance.Button.onClick.RemoveAllListeners();
        characterButtonInstance.Button.onClick.AddListener(() => CharacterSelectedCallback(index));
    }

    private void CharacterSelectedCallback(int index)
    {
        selectedCharacterIndex = index;

        CharacterDataSO characterData = characterDatas[index];

        if(unlockedStates[index])
        {
            lastSelectedCharacterIndex = index;
            characterInfo.Button.interactable = false;
            Save();

            onCharacterSelected?.Invoke(characterData);
        }
        else
        {
            characterInfo.Button.interactable = 
                CurrencyManager.instance.HasEnoughPremiumCurrency(characterData.PurchasePrice);
        }

        centerCharacterImage.sprite = characterData.Sprite;
        characterInfo.Configure(characterData, unlockedStates[index]);
    }

    private void PurchaseSelectedCharacter()
    {
        int price = characterDatas[selectedCharacterIndex].PurchasePrice;
        CurrencyManager.instance.UsePremiumCurrency(price);

        // Save the unlocked state of that character
        unlockedStates[selectedCharacterIndex] = true;

        // Update the concerned character visuals
        characterButtonsParent.GetChild(selectedCharacterIndex).GetComponent<CharacterButton>().Unlock();

        // Update the character info
        CharacterSelectedCallback(selectedCharacterIndex);

        Save();
    }

    public void Load()
    {
        characterDatas = ResourcesManager.Characters;

        for (int i = 0; i < characterDatas.Length; i++)
            unlockedStates.Add(i == 0);

        if(Sijil.TryLoad(this, unlockedStatesKey, out object unlockedStatesObject))
            unlockedStates = (List<bool>)unlockedStatesObject;

        if (Sijil.TryLoad(this, lastSelectedCharacterKey, out object lastSelectedCharacterObject))
            lastSelectedCharacterIndex = (int)lastSelectedCharacterObject;

        Initialize();

        //CharacterSelectedCallback(lastSelectedCharacterIndex);
    }

    public void Save()
    {
        Sijil.Save(this, unlockedStatesKey, unlockedStates);
        Sijil.Save(this, lastSelectedCharacterKey, lastSelectedCharacterIndex);
    }
}
