using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private static readonly IReadOnlyList<ResourceType> ResourceTypes = Array.AsReadOnly(
        (ResourceType[])Enum.GetValues(typeof(ResourceType)));
    private readonly Dictionary<ResourceType, int> _collectedResources = new Dictionary<ResourceType, int>();

    public event Action<ResourceType, int> OnResourceCountChanged;
    public IReadOnlyList<ResourceType> AvailableTypes => ResourceTypes;
    public int TotalResourceCount => _collectedResources.Values.Sum();

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
        if (_collectedResources.ContainsKey(type) == false)
        {
            return;
        }

        int newCount = ++_collectedResources[type];
        OnResourceCountChanged?.Invoke(type, newCount);
    }

    public bool TrySpendResources(int amount)
    {
        if (amount <= 0)
        {
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
