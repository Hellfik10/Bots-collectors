using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _spawnZone;
    [SerializeField] private ResourceSpawnConfig[] _resourceConfigs = new ResourceSpawnConfig[0];

    [Header("Настройки зоны спавна")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(30f, 15f);
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _initialPoolSize = 5;

    private readonly Dictionary<ResourceType, ResourceSpawnConfig> _configsByType =
        new Dictionary<ResourceType, ResourceSpawnConfig>();
    private readonly Dictionary<ResourceType, ResourcePool<Resource>> _pools =
        new Dictionary<ResourceType, ResourcePool<Resource>>();
    private readonly List<ResourceType> _spawnableTypes = new List<ResourceType>();
    private WaitForSeconds _spawnIntervalWait;

    private static readonly WaitForSeconds InitialDelay = new WaitForSeconds(1f);

    private void Awake()
    {
        _spawnIntervalWait = new WaitForSeconds(_spawnInterval);

        for (int i = 0; i < _resourceConfigs.Length; i++)
        {
            InitializePool(_resourceConfigs[i]);
        }
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
            ResourceType type = _spawnableTypes[UnityEngine.Random.Range(0, _spawnableTypes.Count)];
            Spawn(type, GetRandomSpawnPosition());
            yield return _spawnIntervalWait;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 zoneCenter = _spawnZone.position;
        return new Vector3(
            zoneCenter.x + UnityEngine.Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
            0.5f,
            zoneCenter.z + UnityEngine.Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f)
        );
    }

    public void ReturnResource(Resource resource)
    {
        if (resource == null)
        {
            return;
        }

        if (_pools.TryGetValue(resource.Type, out ResourcePool<Resource> pool) == false)
        {
            Destroy(resource.gameObject);
            return;
        }

        resource.transform.SetParent(transform);
        resource.gameObject.SetActive(false);
        pool.Store(resource);
    }

    private Resource Spawn(ResourceType type, Vector3 position)
    {
        if (_configsByType.TryGetValue(type, out ResourceSpawnConfig config) == false)
        {
            return null;
        }

        if (_pools.TryGetValue(type, out ResourcePool<Resource> pool) == false)
        {
            return null;
        }

        Resource resource;
        if (pool.TryTake(out resource) == false)
        {
            resource = Instantiate(config.Prefab, transform);
            resource.Initialize(type);
        }

        resource.transform.SetParent(transform);
        resource.transform.SetPositionAndRotation(position, Quaternion.identity);
        resource.transform.localScale = Vector3.one;
        resource.gameObject.SetActive(true);
        return resource;
    }

    private void InitializePool(ResourceSpawnConfig config)
    {
        _configsByType.Add(config.Type, config);
        _spawnableTypes.Add(config.Type);

        ResourcePool<Resource> pool = new ResourcePool<Resource>();
        _pools.Add(config.Type, pool);

        for (int i = 0; i < _initialPoolSize; i++)
        {
            Resource resource = Instantiate(config.Prefab, transform);
            resource.Initialize(config.Type);
            resource.gameObject.SetActive(false);
            pool.Store(resource);
        }
    }
}
