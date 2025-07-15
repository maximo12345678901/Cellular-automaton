using UnityEngine;
using TMPro;
using System.Text;

public class InformationDisplay : MonoBehaviour
{
    public TextMeshProUGUI rulesTextMeshPro, spawnRarityTextMeshPro;
    public CellManaging cellManagingScript;

    private int selectedRuleIndex = -1;
    private int selectedRarityIndex = 0; // 0 to 3 for rarity1 to rarity4
    private string currentInput = "";
    private bool editingSpawnRarity = false;

    void Update()
    {
        DisplayRules();
        DisplaySpawnRarities();
        HandleNavigation();
        HandleEditing();
    }

    void DisplayRules()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cellManagingScript.rules.Length; i++)
        {
            string rule = cellManagingScript.rules[i];
            if (!editingSpawnRarity && i == selectedRuleIndex)
            {
                sb.AppendLine($"<mark=#ffcc0077><b>{rule}</b></mark>");
            }
            else
            {
                sb.AppendLine(rule);
            }
        }

        rulesTextMeshPro.text = sb.ToString().TrimEnd();
    }

    void DisplaySpawnRarities()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 4; i++)
        {
            int rarity = GetRarityByIndex(i);
            string label = $"spawnRarity{i + 1}: {rarity}";
            if (editingSpawnRarity && i == selectedRarityIndex)
            {
                sb.AppendLine($"<mark=#ccffdd77><b>{label}</b></mark>");
            }
            else
            {
                sb.AppendLine(label);
            }
        }

        spawnRarityTextMeshPro.text = sb.ToString().TrimEnd();
    }

    void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            editingSpawnRarity = !editingSpawnRarity;
            selectedRuleIndex = editingSpawnRarity ? -1 : 0;
            selectedRarityIndex = 0;
            currentInput = "";
        }

        if (editingSpawnRarity)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedRarityIndex = Mathf.Max(0, selectedRarityIndex - 1);
                currentInput = "";
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedRarityIndex = Mathf.Min(3, selectedRarityIndex + 1);
                currentInput = "";
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedRuleIndex = Mathf.Max(0, selectedRuleIndex - 1);
                currentInput = "";
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedRuleIndex = Mathf.Min(cellManagingScript.rules.Length - 1, selectedRuleIndex + 1);
                currentInput = "";
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedRuleIndex = -1;
                currentInput = "";
            }
        }
    }

    void HandleEditing()
    {
        foreach (char c in Input.inputString)
        {
            if (!char.IsDigit(c)) continue;
            currentInput += c;

            if (editingSpawnRarity)
            {
                if (Input.GetKeyDown(KeyCode.Return) || currentInput.Length >= 3)
                {
                    if (int.TryParse(currentInput, out int value))
                    {
                        value = Mathf.Max(1, value);
                        SetRarityByIndex(selectedRarityIndex, value);
                    }
                    currentInput = "";
                }
            }
            else if (selectedRuleIndex != -1)
            {
                if (currentInput.Length == 3)
                {
                    cellManagingScript.rules[selectedRuleIndex] = currentInput;
                    currentInput = "";
                }
            }
        }
    }

    int GetRarityByIndex(int index)
    {
        return index switch
        {
            0 => cellManagingScript.spawnRarity1,
            1 => cellManagingScript.spawnRarity2,
            2 => cellManagingScript.spawnRarity3,
            3 => cellManagingScript.spawnRarity4,
            _ => 10,
        };
    }

    void SetRarityByIndex(int index, int value)
    {
        switch (index)
        {
            case 0: cellManagingScript.spawnRarity1 = value; break;
            case 1: cellManagingScript.spawnRarity2 = value; break;
            case 2: cellManagingScript.spawnRarity3 = value; break;
            case 3: cellManagingScript.spawnRarity4 = value; break;
        }
    }
}
