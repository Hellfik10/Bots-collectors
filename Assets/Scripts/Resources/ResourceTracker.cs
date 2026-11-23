using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    private List<Resource> _freeResources = new List<Resource>();
    private List<Resource> _busyResources = new List<Resource>();

    public bool HasFreeResources => _freeResources.Count > 0;
    public int FreeResourcesCount => _freeResources.Count;
    public int BusyResourcesCount => _busyResources.Count;

    public bool RegisterResource(Resource resource)
    {
        if (resource == null)
        {
            Debug.LogWarning("[ResourceTracker] Попытка зарегистрировать null-ресурс");
            return false;
        }

        if (_freeResources.Contains(resource) || _busyResources.Contains(resource))
        {
            return false;
        }

        _freeResources.Add(resource);
        return true;
    }

    public Resource GetFreeResource()
    {
        if (_freeResources.Count == 0)
        {
            return null;
        }

        Resource resource = _freeResources[0];
        _freeResources.RemoveAt(0);
        _busyResources.Add(resource);

        return resource;
    }

    public void RemoveBusyResource(Resource resource)
    {
        if (resource == null)
        {
            return;
        }

        if (_busyResources.Remove(resource) == false)
        {
            Debug.LogWarning($"[ResourceTracker] Ресурс {resource.name} не найден в списке занятых");
        }
    }

    public void Reset()
    {
        _freeResources.Clear();
        _busyResources.Clear();
    }
}