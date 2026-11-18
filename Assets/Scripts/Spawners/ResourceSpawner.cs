using System;
using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _spawnZone;
    [SerializeField] private ResourcePool _pool;

    [Header("Настройки зоны спавна")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(30f, 15f);
    [SerializeField] private float _spawnInterval = 2f;

    private static readonly ResourceType[] ResourceTypes = (ResourceType[])Enum.GetValues(typeof(ResourceType));

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (enabled)
        {
            SpawnResource();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnResource()
    {
        Vector3 zoneCenter = _spawnZone.position;
        Vector3 spawnPosition = new Vector3(
            zoneCenter.x + UnityEngine.Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
            0.5f,
            zoneCenter.z + UnityEngine.Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f)
        );
        ResourceType type = ResourceTypes[UnityEngine.Random.Range(0, ResourceTypes.Length)];

        _pool.Get(type, spawnPosition);
    }
}
