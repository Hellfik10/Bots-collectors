using UnityEngine;

[RequireComponent(typeof(BaseResourceStorage))]
public class BaseResourceCollector : MonoBehaviour
{
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private ResourceSpawner _spawner;

    private BaseResourceStorage _storage;

    internal ResourceTracker Tracker => _tracker;

    private void Awake()
    {
        _storage = GetComponent<BaseResourceStorage>();
    }

    public void CollectResource(Resource resource)
    {
        ResourceType type = resource.Type;
        _tracker.RemoveBusyResource(resource);
        _spawner.ReturnResource(resource);
        _storage.AddResource(type);
    }
}
