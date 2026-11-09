using UnityEngine;
using System;
using System.Collections.Generic;

public class Base : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private ResourcePool _pool;

    [Header("Настройки")]
    [SerializeField] private int _startingUnitsCount = 3;

    private List<Unit> _freeUnits = new List<Unit>();
    private Dictionary<ResourceType, int> _collectedResources = new Dictionary<ResourceType, int>();

    private int _startResourceCount = 0;
    private int _botSpawnPositionY = 1;

    public event Action<ResourceType, int> OnResourceCountChanged;

    private void Start()
    {
        _collectedResources[ResourceType.Coal] = _startResourceCount;
        _collectedResources[ResourceType.Iron] = _startResourceCount;
        _collectedResources[ResourceType.Copper] = _startResourceCount;

        SpawnInitialUnits();
    }

    private void SpawnInitialUnits()
    {
        for (int i = 0; i < _startingUnitsCount; i++)
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * 2f;
            spawnPosition.y = _botSpawnPositionY;

            GameObject unitObj = Instantiate(_unitPrefab, spawnPosition, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            
            unit.Initialize(this);
            unit.OnBecameFree += OnUnitBecameFree;
            
            _freeUnits.Add(unit);
        }
    }

    private void OnUnitBecameFree(Unit unit)
    {
        if (!_freeUnits.Contains(unit))
        {
            _freeUnits.Add(unit);
        }
        TryAssignTask();
    }

    public void CollectResource(Resource resource)
    {
        ResourceType type = resource.Type;
        _collectedResources[type]++;
        int newCount = _collectedResources[type];
        
        _tracker.MarkAsCollected(resource);
        _pool.Return(resource.gameObject);

        OnResourceCountChanged?.Invoke(type, newCount);
    }

    public void TryAssignTask()
    {
        if (_freeUnits.Count == 0) return;
        if (!_tracker.HasFreeResources) return;

        Resource freeResource = _tracker.GetFreeResource();
        if (freeResource == null) return;

        Unit freeUnit = _freeUnits[0];
        _freeUnits.RemoveAt(0);

        freeUnit.AssignTask(freeResource);
    }
}