using UnityEngine;
using TMPro;
using System.Text;

public class InformationDisplay : MonoBehaviour
{
    public TextMeshProUGUI rulesTextMeshPro;
    public CellManaging cellManagingScript;

    private int selectedRuleIndex = -1;

    private string currentInput = "";

    void Update()
    {
        DisplayRules();
        HandleNavigation();
        HandleEditing();
    }

    void DisplayRules()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cellManagingScript.rules.Length; i++)
        {
            string rule = cellManagingScript.rules[i];

            if (i == selectedRuleIndex)
            {
                sb.Append($"<mark=#ffcc0077><b>{rule}</b></mark>  ");
            }
            else
            {
                sb.Append($"{rule}   ");
            }

            if (i % 2 == 1)
            {
                sb.AppendLine();
            }
        }

        rulesTextMeshPro.text = sb.ToString().TrimEnd();
    }

    void HandleNavigation()
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

    void HandleEditing()
    {
        foreach (char c in Input.inputString)
        {
            if (!char.IsDigit(c)) continue;
            currentInput += c;

            if (selectedRuleIndex != -1)
            {
                if (currentInput.Length == 4)
                {
                    cellManagingScript.rules[selectedRuleIndex] = currentInput;
                    currentInput = "";
                }
            }
        }
    }
}
