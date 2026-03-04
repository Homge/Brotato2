using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tabsil.Sijil;

public class CurrencyManager : MonoBehaviour, IWantToBeSaved
{
    public static CurrencyManager instance;

    private const string premiumCurrencyKey = "PremiumCurrency";

    [field: SerializeField] public int Currency { get; private set; }
    [field: SerializeField] public int PremiumCurrency { get; private set; }

    [Header(" Actions ")]
    public static Action onUpdated;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //AddPremiumCurrency(PlayerPrefs.GetInt(premiumCurrencyKey, 100), false);

        Candy.onCollected += CandyCollectedCallback;
        Cash.onCollected += CashCollectedCallback;
    }

    private void OnDestroy()
    {
        Candy.onCollected -= CandyCollectedCallback;
        Cash.onCollected -= CashCollectedCallback;
    }

    public void Load()
    {
        if (Sijil.TryLoad(this, premiumCurrencyKey, out object premiumCurrencyValue))
            AddPremiumCurrency((int)premiumCurrencyValue, false);
        else
            AddPremiumCurrency(100, false);
    }

    public void Save()
    {
        Sijil.Save(this, premiumCurrencyKey, PremiumCurrency);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateTexts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [NaughtyAttributes.Button]
    private void Add500Currency() => AddCurrency(500);

    [NaughtyAttributes.Button]
    private void Add500PCurrency() => AddPremiumCurrency(500);

    public void AddCurrency(int amount)
    {
        Currency += amount;
        UpdateVisuals();
    }

    public void AddPremiumCurrency(int amount, bool save = true)
    {
        PremiumCurrency += amount;
        UpdateVisuals();

        //PlayerPrefs.SetInt(premiumCurrencyKey, PremiumCurrency);
    }

    private void UpdateVisuals()
    {
        UpdateTexts();
        onUpdated?.Invoke();
        Save();
    }



    private void UpdateTexts()
    {
        CurrencyText[] currencyTexts = FindObjectsByType<CurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyText text in currencyTexts)
            text.UpdateText(Currency.ToString());  
        
        PremiumCurrencyText[] premiumCurrencyTexts = FindObjectsByType<PremiumCurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (PremiumCurrencyText text in premiumCurrencyTexts)
            text.UpdateText(PremiumCurrency.ToString());
    }

    public void UseCurrency(int price) => AddCurrency(-price);
    public void UsePremiumCurrency(int price) => AddPremiumCurrency(-price);

    public bool HasEnoughCurrency(int price) => Currency >= price;
    public bool HasEnoughPremiumCurrency(int price) => PremiumCurrency >= price;

    private void CandyCollectedCallback(Candy candy) => AddCurrency(1);
    private void CashCollectedCallback(Cash cash) => AddPremiumCurrency(1);


}
