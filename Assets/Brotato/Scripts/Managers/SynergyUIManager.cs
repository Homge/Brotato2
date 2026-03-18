using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SynergyUIManager : MonoBehaviour
{
    [SerializeField] private PlayerWeapons playerWeapons;
    [SerializeField] private TextMeshProUGUI synergyTextDisplay;

    // Hàm này sẽ tự động chạy mỗi khi Panel chứa nó được bật lên (ví dụ: Mở Inventory)
    private void OnEnable()
    {
        UpdateSynergyUI();
    }

    public void UpdateSynergyUI()
    {
        if (playerWeapons == null || synergyTextDisplay == null) return;

        Dictionary<WeaponTag, int> tagCounts = playerWeapons.GetTagCounts();
        SynergyDefinition[] synergies = playerWeapons.GetAvailableSynergies();

        string displayText = "<b>Synergies</b>\n";

        foreach (SynergyDefinition syn in synergies)
        {
            int currentCount = tagCounts.ContainsKey(syn.requiredTag) ? tagCounts[syn.requiredTag] : 0;

            // Chỉ hiển thị những bộ Synergy mà người chơi đang có ít nhất 1 vũ khí
            if (currentCount > 0)
            {
                if (currentCount >= syn.requiredCount)
                {
                    // Đạt mốc kích hoạt (In màu xanh)
                    displayText += $"<color=#00FF00>{syn.synergyName}: {currentCount}/{syn.requiredCount} (Kích hoạt)</color>\n";
                }
                else
                {
                    // Chưa đạt mốc (In màu trắng)
                    displayText += $"<color=#FFFFFF>{syn.synergyName}: {currentCount}/{syn.requiredCount}</color>\n";
                }
            }
        }

        synergyTextDisplay.text = displayText;
    }
}