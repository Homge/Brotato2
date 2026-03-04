using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatContainerManager : MonoBehaviour
{
    public static StatContainerManager instance;

    [Header(" Elements ")]
    [SerializeField] private StatContainer statContainer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void GenerateContainers(Dictionary<Stat, float> statDictionnary, Transform parent)
    {
        List<StatContainer> statContainers = new List<StatContainer>();

        foreach (KeyValuePair<Stat, float> kvp in statDictionnary)
        {
            StatContainer containerInstance = Instantiate(statContainer, parent);
            statContainers.Add(containerInstance);

            Sprite icon = ResourcesManager.GetStatIcon(kvp.Key);
            string statName = Enums.FormatStatName(kvp.Key);
            float statValue = kvp.Value;

            containerInstance.Configure(icon, statName, statValue);
        }

        LeanTween.delayedCall(Time.deltaTime * 2, () => ResizeTexts(statContainers));
    }

    private void ResizeTexts(List<StatContainer> statContainers)
    {
        float minFontSize = 5000;

        for (int i = 0; i < statContainers.Count; i++)
        {
            StatContainer statContainer = statContainers[i];
            float fontSize = statContainer.GetFontSize();

            if(fontSize < minFontSize)
                minFontSize = fontSize;
        }

        // At this pointhave the min font size setup
        // Set this font size for all of the stat name texts

        for (int i = 0; i < statContainers.Count; i++)
            statContainers[i].SetFontSize(minFontSize);        
    }

    public static void GenerateStatContainers(Dictionary<Stat, float> statDictionnary, Transform parent)
    {
        parent.Clear();
        instance.GenerateContainers(statDictionnary, parent);
    }
}
