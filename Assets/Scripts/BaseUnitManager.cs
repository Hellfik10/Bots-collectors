using System.Collections.Generic;
using UnityEngine;

public class BaseUnitManager : MonoBehaviour
{
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private int _startingUnitsCount = 3;

    private readonly List<Unit> _freeUnits = new List<Unit>();
    private Base _homeBase;

    private void Awake()
    {
        _homeBase = GetComponent<Base>();
    }

    private void Start()
    {
        SpawnInitialUnits();
    }

    private void Update()
    {
        TryAssignTask();
    }

    public void TryAssignTask()
    {
        if (_freeUnits.Count == 0 || !_tracker.HasFreeResources)
        {
            return;
        }

        Resource resource = _tracker.GetFreeResource();
        if (resource == null)
        {
            return;
        }

        Unit unit = _freeUnits[0];
        _freeUnits.RemoveAt(0);
        unit.AssignTask(resource);
    }

    private void SpawnInitialUnits()
    {
        for (int i = 0; i < _startingUnitsCount; i++)
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * 2f;
            spawnPosition.y = 1f;

            Unit unit = Instantiate(_unitPrefab, spawnPosition, Quaternion.identity);
            unit.Initialize(_homeBase);
            unit.OnBecameFree += OnUnitBecameFree;
            _freeUnits.Add(unit);
        }

        TryAssignTask();
    }

    private void OnUnitBecameFree(Unit unit)
    {
        if (!_freeUnits.Contains(unit))
        {
            _freeUnits.Add(unit);
        }

        TryAssignTask();
    }
}
