using UnityEngine;
using TMPro;
using System;

public class ResourceUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Base _base;

    [Header("Текстовые поля")]
    [SerializeField] private TextMeshProUGUI _coalText;
    [SerializeField] private TextMeshProUGUI _ironText;
    [SerializeField] private TextMeshProUGUI _copperText;

    private void Start()
    {
         _base.OnResourceCountChanged += HandleResourceCountChanged;

        _coalText.text = $"Уголь: 0";
        _ironText.text = $"Железо: 0";
        _copperText.text = $"Медь: 0";
    }

    private void OnDisable()
    {
        _base.OnResourceCountChanged -= HandleResourceCountChanged;
    }

    private void HandleResourceCountChanged(ResourceType type, int newCount)
    {
        switch (type)
        {
            case ResourceType.Coal:
                _coalText.text = $"Уголь: {newCount}";
                break;
            case ResourceType.Iron:
                _ironText.text = $"Железо: {newCount}";
                break;
            case ResourceType.Copper:
                _copperText.text = $"Медь: {newCount}";
                break;
        }
    }
}