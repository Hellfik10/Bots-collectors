using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ResourceScanner), typeof(BaseUnitManager), typeof(BaseResourceStorage))]
public class Base : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private ResourceSpawner _spawner;
    [SerializeField] private Collider _mapCollider;

    private BaseUnitManager _unitManager;
    private BaseResourceStorage _storage;
    private static Base _selectedBase;

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
        if (_selectedBase == this)
        {
            _selectedBase = null;
        }
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Base clickedBase = null;
        float closestBaseHit = Mathf.Infinity;
        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            Base hitBase = hit.collider.GetComponentInParent<Base>();
            if (hitBase != null && hit.distance < closestBaseHit)
            {
                clickedBase = hitBase;
                closestBaseHit = hit.distance;
            }
        }

        if (clickedBase != null)
        {
            _selectedBase = clickedBase;
            return;
        }

        if (_selectedBase != this)
        {
            return;
        }

        if (_mapCollider == null)
        {
            Debug.LogError("[Base] Не задан коллайдер карты для размещения флага", this);
            return;
        }

        if (_mapCollider.Raycast(ray, out RaycastHit mapHit, Mathf.Infinity))
        {
            _unitManager.SetExpansionFlag(mapHit.point);
        }
    }

    public int GetResourceCount(ResourceType type)
    {
        return Storage.GetResourceCount(type);
    }

    public int TotalResourceCount => Storage.TotalResourceCount;

    public bool TrySpendResources(int amount)
    {
        return Storage.TrySpendResources(amount);
    }

    public void CollectResource(Resource resource)
    {
        ResourceType type = resource.Type;
        _tracker.MarkAsCollected(resource);
        _spawner.ReturnResource(resource);
        _storage.AddResource(type);
    }

    public void InitializeFoundedBase(Unit founder)
    {
        _unitManager.InitializeFoundedBase(founder);
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
