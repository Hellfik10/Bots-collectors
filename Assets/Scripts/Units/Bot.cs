using UnityEngine;
using System;

[RequireComponent(typeof(Mover))]
public class Unit : MonoBehaviour
{
    private Vector3 _carryOffset = new Vector3(0f, 1.2f, 0.3f);
    private Resource _carriedResource;
    private BaseResourceCollector _homeBase;
    private Mover _mover;
    private Action<Unit> _onBuildSiteReached;

    public event Action<Unit> OnBecameFree;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    private void OnDestroy()
    {
        ClearDestinationHandlers();
    }

    public void Initialize(BaseResourceCollector homeBase)
    {
        _homeBase = homeBase;
    }

    public void TransferToBase(BaseResourceCollector homeBase)
    {
        ClearDestinationHandlers();
        _mover.StopMoving();
        _homeBase = homeBase;
        _carriedResource = null;
        _onBuildSiteReached = null;
    }

    public void AssignTask(Resource resource)
    {
        ClearDestinationHandlers();
        _carriedResource = resource;
        _onBuildSiteReached = null;
        _mover.DestinationReached += HandleResourceReached;
        _mover.MoveTo(resource.transform);
    }

    public void AssignBuildTask(Transform flag, Action<Unit> onBuildSiteReached)
    {
        ClearDestinationHandlers();
        _carriedResource = null;
        _onBuildSiteReached = onBuildSiteReached;
        _mover.DestinationReached += HandleBuildSiteReached;
        _mover.MoveTo(flag);
    }

    private void HandleResourceReached()
    {
        _mover.DestinationReached -= HandleResourceReached;
        _carriedResource.transform.SetParent(transform);
        _carriedResource.transform.localPosition = _carryOffset;

        _mover.DestinationReached += HandleHomeBaseReached;
        _mover.MoveTo(_homeBase.transform);
    }

    private void HandleHomeBaseReached()
    {
        _mover.DestinationReached -= HandleHomeBaseReached;
        _homeBase.CollectResource(_carriedResource);
        _carriedResource = null;
        OnBecameFree?.Invoke(this);
    }

    private void HandleBuildSiteReached()
    {
        _mover.DestinationReached -= HandleBuildSiteReached;
        Action<Unit> callback = _onBuildSiteReached;
        _onBuildSiteReached = null;
        callback?.Invoke(this);
    }

    private void ClearDestinationHandlers()
    {
        _mover.DestinationReached -= HandleResourceReached;
        _mover.DestinationReached -= HandleHomeBaseReached;
        _mover.DestinationReached -= HandleBuildSiteReached;
    }
}