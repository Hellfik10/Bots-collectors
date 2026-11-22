using System.Collections.Generic;
using UnityEngine;

public class BaseUnitManager : MonoBehaviour
{
    private const int ResourcesPerUnit = 3;
    private const int ResourcesPerBase = 5;

    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private int _startingUnitsCount = 3;

    private readonly List<Unit> _freeUnits = new List<Unit>();
    private Base _homeBase;
    private GameObject _expansionFlag;
    private int _ownedUnitsCount;
    private bool _spawnInitialUnits = true;
    private bool _expansionInProgress;

    private void Awake()
    {
        _homeBase = GetComponent<Base>();
    }

    private void Start()
    {
        if (_spawnInitialUnits)
        {
            SpawnInitialUnits();
        }
    }

    private void Update()
    {
        TryAssignTask();
    }

    public void TryAssignTask()
    {
        if (_expansionFlag != null)
        {
            TryAssignExpansionTask();
            return;
        }

        while (_homeBase.TotalResourceCount >= ResourcesPerUnit)
        {
            if (!_homeBase.TrySpendResources(ResourcesPerUnit))
            {
                break;
            }

            SpawnUnit();
        }

        AssignResourceTask();
    }

    public void SetExpansionFlag(Vector3 position)
    {
        if (_expansionFlag == null)
        {
            _expansionFlag = CreateExpansionFlag(position);
        }
        else
        {
            _expansionFlag.transform.position = position;
        }

        TryAssignTask();
    }

    public void InitializeFoundedBase(Unit founder)
    {
        _spawnInitialUnits = false;
        _ownedUnitsCount = 1;
        RegisterFreeUnit(founder);
        founder.TransferToBase(_homeBase);
    }

    private void TryAssignExpansionTask()
    {
        if (_expansionInProgress)
        {
            return;
        }

        if (_homeBase.TotalResourceCount >= ResourcesPerBase)
        {
            if (_ownedUnitsCount > 1 && _freeUnits.Count > 0 &&
                _homeBase.TrySpendResources(ResourcesPerBase))
            {
                Unit founder = TakeFreeUnit();
                _expansionInProgress = true;
                founder.AssignBuildTask(_expansionFlag.transform, BuildExpansionBase);
            }

            return;
        }

        AssignResourceTask();
    }

    private void AssignResourceTask()
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

        TakeFreeUnit().AssignTask(resource);
    }

    private Unit TakeFreeUnit()
    {
        Unit unit = _freeUnits[0];
        _freeUnits.RemoveAt(0);
        return unit;
    }

    private void SpawnInitialUnits()
    {
        for (int i = 0; i < _startingUnitsCount; i++)
        {
            SpawnUnit();
        }

        TryAssignTask();
    }

    private void SpawnUnit()
    {
        Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * 2f;
        spawnPosition.y = 1f;

        Unit unit = Instantiate(_unitPrefab, spawnPosition, Quaternion.identity);
        unit.Initialize(_homeBase);
        _ownedUnitsCount++;
        RegisterFreeUnit(unit);
    }

    private void RegisterFreeUnit(Unit unit)
    {
        unit.OnBecameFree -= OnUnitBecameFree;
        unit.OnBecameFree += OnUnitBecameFree;
        if (!_freeUnits.Contains(unit))
        {
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

    private void BuildExpansionBase(Unit founder)
    {
        Vector3 position = _expansionFlag.transform.position;
        position.y = _homeBase.transform.position.y;
        Base newBase = Instantiate(_homeBase, position, Quaternion.identity);
        GameObject completedFlag = _expansionFlag;

        founder.OnBecameFree -= OnUnitBecameFree;
        _ownedUnitsCount--;
        _expansionFlag = null;
        _expansionInProgress = false;

        newBase.InitializeFoundedBase(founder);
        Destroy(completedFlag);
        TryAssignTask();
    }

    private static GameObject CreateExpansionFlag(Vector3 position)
    {
        GameObject flag = new GameObject("BaseExpansionFlag");
        flag.transform.position = position;

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.SetParent(flag.transform, false);
        pole.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        pole.transform.localScale = new Vector3(0.08f, 0.65f, 0.08f);
        pole.GetComponent<Collider>().enabled = false;
        pole.GetComponent<Renderer>().material.color = Color.gray;

        GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        banner.name = "Banner";
        banner.transform.SetParent(flag.transform, false);
        banner.transform.localPosition = new Vector3(0.22f, 1.05f, 0f);
        banner.transform.localScale = new Vector3(0.4f, 0.25f, 0.04f);
        banner.GetComponent<Collider>().enabled = false;
        banner.GetComponent<Renderer>().material.color = Color.red;

        return flag;
    }
}
