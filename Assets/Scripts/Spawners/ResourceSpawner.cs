using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Base _base;
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private Transform _spawnZone;
    [SerializeField] private ResourcePool _pool;

    [Header("Настройки зоны спавна")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(30f, 15f);
    [SerializeField] private float _spawnInterval = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnResource), 1f, _spawnInterval);
    }

    private void SpawnResource()
    {
        Vector3 zoneCenter = _spawnZone.position;

        Vector3 spawnPos = new Vector3(
            zoneCenter.x + Random.Range(-_spawnAreaSize.x / 2, _spawnAreaSize.x / 2),
            0,
            zoneCenter.z + Random.Range(-_spawnAreaSize.y / 2, _spawnAreaSize.y / 2)
        );

        ResourceType randomType = (ResourceType)Random.Range(0, 3);

        GameObject resObj = _pool.Get(randomType, spawnPos);

        if (resObj == null)
        {
            return;
        }

        Resource resource = resObj.GetComponent<Resource>();

        if (resource == null)
        {
            _pool.Return(resObj);
            return;
        }

        _tracker.RegisterResource(resource);
        _base.TryAssignTask();
    }
}