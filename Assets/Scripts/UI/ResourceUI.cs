using UnityEngine;
using TMPro;
using System;

public class ResourceUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Base _base;

    [Header("Текстовое поле")]
    [SerializeField] private TextMeshProUGUI _label;

    private void OnEnable()
    {
        if (_base != null && _label != null)
        {
            _base.OnResourceCountChanged += HandleResourceCountChanged;
            UpdateState();
        }
    }

    private void OnDisable()
    {
        if (_base != null)
        {
            _base.OnResourceCountChanged -= HandleResourceCountChanged;
        }
    }

    private void HandleResourceCountChanged(ResourceType type, int newCount)
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (_base == null || _label == null) return;

        bool hasAnyResources = false;
        string displayText = "";

        var types = _base.AvailableTypes;
        
        for (int i = 0; i < types.Count; i++)
        {
            ResourceType type = types[i];
            int count = _base.GetResourceCount(type);

            if (count > 0)
            {
                hasAnyResources = true;
            }

            if (i > 0)
            {
                displayText += "\n";
            }
            displayText += $"{type}: {count}";
        }

        _label.gameObject.SetActive(hasAnyResources);

        if (hasAnyResources)
        {
            _label.text = displayText;
        }
    }
}