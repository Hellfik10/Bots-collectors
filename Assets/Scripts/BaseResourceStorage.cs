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
}
