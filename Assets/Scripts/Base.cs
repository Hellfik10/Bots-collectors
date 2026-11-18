using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner), typeof(BaseUnitManager), typeof(BaseResourceStorage))]
public class Base : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private ResourcePool _pool;

    private BaseUnitManager _unitManager;
    private BaseResourceStorage _storage;

    public event Action<ResourceType, int> OnResourceCountChanged;
    public IReadOnlyList<ResourceType> AvailableTypes => Storage.AvailableTypes;
    private BaseResourceStorage Storage => _storage != null ? _storage : GetComponent<BaseResourceStorage>();

    private void Awake()
    {
        _unitManager = GetComponent<BaseUnitManager>();
        _storage = GetComponent<BaseResourceStorage>();
        _storage.OnResourceCountChanged += HandleResourceCountChanged;
    }

    private void OnDestroy()
    {
        _storage.OnResourceCountChanged -= HandleResourceCountChanged;
    }

    public int GetResourceCount(ResourceType type)
    {
        return Storage.GetResourceCount(type);
    }

    public void CollectResource(Resource resource)
    {
        ResourceType type = resource.Type;
        _tracker.MarkAsCollected(resource);
        _pool.Return(resource.gameObject);
        _storage.AddResource(type);
    }

    public void TryAssignTask()
    {
        _unitManager.TryAssignTask();
    }

    private void HandleResourceCountChanged(ResourceType type, int newCount)
    {
        OnResourceCountChanged?.Invoke(type, newCount);
    }
}
