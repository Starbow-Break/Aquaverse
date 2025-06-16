using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartLineUI : MonoBehaviour
{
    public PartType partType;

    [SerializeField] private TMP_Text partNameText;
    [SerializeField] private TMP_Text indexText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private List<MeshPartOption> options;
    private int currentIndex = 0;

    public void Init(PartType type, List<MeshPartOption> options)
    {
        partType = type;
        partNameText.text = type.ToString();

        this.options = options;
        currentIndex = 0;

        UpdateUI();

        leftButton.onClick.AddListener(OnLeft);
        rightButton.onClick.AddListener(OnRight);
    }
    
    public void RefreshUI()
    {
        if (CustomizationManager.Instance.CurrentSelections.TryGetValue(partType, out var selectedOption))
        {
            currentIndex = options.FindIndex(opt => opt.id == selectedOption.id);
            if (currentIndex == -1) currentIndex = 0;
        }
        else
        {
            currentIndex = 0;
        }

        UpdateUI();
    }

    private void OnLeft()
    {
        if (options.Count == 0) return;
        currentIndex = (currentIndex - 1 + options.Count) % options.Count;
        Apply();
    }

    private void OnRight()
    {
        if (options.Count == 0) return;
        currentIndex = (currentIndex + 1) % options.Count;
        Apply();
    }

    private void Apply()
    {
        CustomizationManager.Instance.ApplyOption(partType, options[currentIndex].id);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (options.Count == 0)
        {
            indexText.text = "0 / 0";
        }
        else
        {
            indexText.text = $"{currentIndex + 1} / {options.Count}";
        }
    }
}