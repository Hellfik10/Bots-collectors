using UnityEngine;
using System;

[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public enum State { Idle, GoingToResource, Returning }

    private Vector3 _carryOffset = new Vector3(0f, 1.2f, 0.3f);
    private State _state = State.Idle;
    private Resource _carriedResource;
    private Base _homeBase;
    private UnitMovement _movement;

    public event Action<Unit> OnBecameFree;

    private void Awake()
    {
        _movement = GetComponent<UnitMovement>();
        _movement.OnDestinationReached += HandleDestinationReached;
    }

    private void OnDestroy()
    {
        _movement.OnDestinationReached -= HandleDestinationReached;
    }

    public void Initialize(Base homeBase)
    {
        _homeBase = homeBase;
    }

    public void AssignTask(Resource resource)
    {
        _carriedResource = resource;
        _state = State.GoingToResource;
        _movement.MoveTo(resource.transform);
    }

    private void HandleDestinationReached()
    {
        if (_state == State.GoingToResource)
        {
            _carriedResource.transform.SetParent(transform);
            _carriedResource.transform.localPosition = _carryOffset;

            _state = State.Returning;
            _movement.MoveTo(_homeBase.transform);
        }
        else if (_state == State.Returning)
        {
            _homeBase.CollectResource(_carriedResource);
            _carriedResource = null;
            _state = State.Idle;
            OnBecameFree?.Invoke(this);
        }
    }
}