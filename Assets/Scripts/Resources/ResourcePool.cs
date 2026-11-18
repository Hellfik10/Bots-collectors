using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [Header("Префабы ресурсов")]
    [SerializeField] private GameObject _coalPrefab;
    [SerializeField] private GameObject _ironPrefab;
    [SerializeField] private GameObject _copperPrefab;

    [Header("Начальное количество объектов в пуле")]
    [SerializeField] private int _initialPoolSize = 5;

    private Dictionary<ResourceType, Queue<GameObject>> _pools = new Dictionary<ResourceType, Queue<GameObject>>();
    private Dictionary<GameObject, ResourceType> _objectToType = new Dictionary<GameObject, ResourceType>();

    public GameObject Get(ResourceType type, Vector3 position)
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null)
        {
            return null;
        }

        GameObject obj;

        if (_pools[type].Count > 0)
        {
            obj = _pools[type].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, transform);
            _objectToType[obj] = type;
        }

        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (!_objectToType.ContainsKey(obj))
        {
            Debug.LogWarning($"[ResourcePool] Объект {obj.name} не принадлежит пулу!");
            Destroy(obj);
            return;
        }

        ResourceType type = _objectToType[obj];

        obj.transform.SetParent(transform);
        obj.SetActive(false);
        _pools[type].Enqueue(obj);
    }

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        _pools[ResourceType.Coal] = new Queue<GameObject>();
        _pools[ResourceType.Iron] = new Queue<GameObject>();
        _pools[ResourceType.Copper] = new Queue<GameObject>();

        CreatePoolObjects(ResourceType.Coal, _coalPrefab);
        CreatePoolObjects(ResourceType.Iron, _ironPrefab);
        CreatePoolObjects(ResourceType.Copper, _copperPrefab);
    }

    private void CreatePoolObjects(ResourceType type, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError($"[ResourcePool] Не задан префаб для ресурса {type}", this);
            return;
        }

        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);

            _pools[type].Enqueue(obj);
            _objectToType[obj] = type;
        }
    }

    private GameObject GetPrefab(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Coal: return _coalPrefab;
            case ResourceType.Iron: return _ironPrefab;
            case ResourceType.Copper: return _copperPrefab;
            default: return null;
        }
    }
}