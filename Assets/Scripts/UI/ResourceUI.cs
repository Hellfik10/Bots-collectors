using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private BaseResourceStorage _storage;
    [SerializeField] private TextMeshProUGUI _label;

    private void OnEnable()
    {
        if (_storage == null || _label == null)
        {
            return;
        }

        _storage.OnResourceCountChanged += UpdateState;

        IReadOnlyList<ResourceType> types = _storage.AvailableTypes;
        if (types.Count == 0)
        {
            _label.text = string.Empty;
            _label.enabled = true;
            return;
        }

        ResourceType firstType = types[0];
        UpdateState(firstType, _storage.GetResourceCount(firstType));
    }

    private void OnDisable()
    {
        if (_storage != null)
        {
            _storage.OnResourceCountChanged -= UpdateState;
        }
    }

    private void UpdateState(ResourceType changedType, int newCount)
    {
        if (_storage == null || _label == null)
        {
            return;
        }

        IReadOnlyList<ResourceType> types = _storage.AvailableTypes;
        string displayText = string.Empty;
        for (int i = 0; i < types.Count; i++)
        {
            ResourceType type = types[i];
            int count = type == changedType ? newCount : _storage.GetResourceCount(type);

            if (i > 0)
            {
                displayText += "\n";
            }

            displayText += $"{type}: {count}";
        }

        _label.text = displayText;
        _label.enabled = true;
    }
}
