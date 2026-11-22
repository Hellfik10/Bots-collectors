using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private static readonly IReadOnlyList<ResourceType> ResourceTypes = Array.AsReadOnly(
        (ResourceType[])Enum.GetValues(typeof(ResourceType)));
    private readonly Dictionary<ResourceType, int> _collectedResources = new Dictionary<ResourceType, int>();

    public event Action<ResourceType, int> OnResourceCountChanged;
    public IReadOnlyList<ResourceType> AvailableTypes => ResourceTypes;
    public int TotalResourceCount
    {
        get
        {
            int total = 0;
            foreach (int count in _collectedResources.Values)
            {
                total += count;
            }

            return total;
        }
    }

    private void Awake()
    {
        foreach (ResourceType type in ResourceTypes)
        {
            _collectedResources[type] = 0;
        }
    }

    public int GetResourceCount(ResourceType type)
    {
        return _collectedResources.TryGetValue(type, out int count) ? count : 0;
    }

    public void AddResource(ResourceType type)
    {
        if (!_collectedResources.ContainsKey(type))
        {
            Debug.LogWarning($"[BaseResourceStorage] Неизвестный тип ресурса: {type}", this);
            return;
        }

        int newCount = ++_collectedResources[type];
        OnResourceCountChanged?.Invoke(type, newCount);
    }

    public bool TrySpendResources(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[BaseResourceStorage] Некорректное количество ресурсов для списания: {amount}", this);
            return false;
        }

        if (TotalResourceCount < amount)
        {
            return false;
        }

        foreach (ResourceType type in ResourceTypes)
        {
            int available = _collectedResources[type];
            int spent = Mathf.Min(available, amount);
            if (spent == 0)
            {
                continue;
            }

            int newCount = _collectedResources[type] - spent;
            _collectedResources[type] = newCount;
            amount -= spent;
            OnResourceCountChanged?.Invoke(type, newCount);
            if (amount == 0)
            {
                break;
            }
        }

        return true;
    }
}
