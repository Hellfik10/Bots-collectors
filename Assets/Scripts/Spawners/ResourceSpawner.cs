using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _spawnZone;
    [SerializeField] private Resource _coalPrefab;
    [SerializeField] private Resource _ironPrefab;
    [SerializeField] private Resource _copperPrefab;

    [Header("Настройки зоны спавна")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(30f, 15f);
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _initialPoolSize = 5;

    private static readonly ResourceType[] ResourceTypes = (ResourceType[])Enum.GetValues(typeof(ResourceType));
    private readonly Dictionary<ResourceType, Resource> _prefabs = new Dictionary<ResourceType, Resource>();
    private readonly Dictionary<ResourceType, ResourcePool<Resource>> _pools =
        new Dictionary<ResourceType, ResourcePool<Resource>>();
    private WaitForSeconds _spawnIntervalWait;

    private void Awake()
    {
        _spawnIntervalWait = new WaitForSeconds(_spawnInterval);
        InitializePool(ResourceType.Coal, _coalPrefab);
        InitializePool(ResourceType.Iron, _ironPrefab);
        InitializePool(ResourceType.Copper, _copperPrefab);
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return InitialDelay;

        while (enabled)
        {
            SpawnResource();
            yield return _spawnIntervalWait;
        }
    }

    private static readonly WaitForSeconds InitialDelay = new WaitForSeconds(1f);

    private void SpawnResource()
    {
        Vector3 zoneCenter = _spawnZone.position;
        Vector3 spawnPosition = new Vector3(
            zoneCenter.x + UnityEngine.Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
            0.5f,
            zoneCenter.z + UnityEngine.Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f)
        );
        ResourceType type = ResourceTypes[UnityEngine.Random.Range(0, ResourceTypes.Length)];
        Spawn(type, spawnPosition);
    }

    public void ReturnResource(Resource resource)
    {
        if (resource == null)
        {
            return;
        }

        if (!_pools.TryGetValue(resource.Type, out ResourcePool<Resource> pool))
        {
            Debug.LogWarning($"[ResourceSpawner] Ресурс {resource.name} не принадлежит спавнеру", resource);
            Destroy(resource.gameObject);
            return;
        }

        resource.transform.SetParent(transform);
        resource.gameObject.SetActive(false);
        pool.Store(resource);
    }

    private Resource Spawn(ResourceType type, Vector3 position)
    {
        if (!_prefabs.TryGetValue(type, out Resource prefab))
        {
            Debug.LogError($"[ResourceSpawner] Не задан префаб для ресурса {type}", this);
            return null;
        }

        ResourcePool<Resource> pool = _pools[type];
        if (!pool.TryTake(out Resource resource))
        {
            resource = Instantiate(prefab, transform);
            resource.Initialize(type);
        }

        resource.transform.SetParent(transform);
        resource.transform.SetPositionAndRotation(position, Quaternion.identity);
        resource.transform.localScale = Vector3.one;
        resource.gameObject.SetActive(true);
        return resource;
    }

    private void InitializePool(ResourceType type, Resource prefab)
    {
        if (prefab == null)
        {
            Debug.LogError($"[ResourceSpawner] Не задан префаб для ресурса {type}", this);
            return;
        }

        _prefabs.Add(type, prefab);
        ResourcePool<Resource> pool = new ResourcePool<Resource>();
        _pools.Add(type, pool);

        for (int i = 0; i < _initialPoolSize; i++)
        {
            Resource resource = Instantiate(prefab, transform);
            resource.Initialize(type);
            resource.gameObject.SetActive(false);
            pool.Store(resource);
        }
    }
}
