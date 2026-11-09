using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public enum State { Idle, GoingToResource, Returning }

    [Header("Настройки движения")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _stopDistance = 0.2f;

    private Vector3 _carryOffset = new Vector3(0f, 1.2f, 0.3f);

    private State _state = State.Idle;
    private Transform _target;
    private Resource _carriedResource;
    private Base _homeBase;

    public event Action<Unit> OnBecameFree;

    public void Initialize(Base homeBase)
    {
        _homeBase = homeBase;
    }

    public void AssignTask(Resource resource)
    {
        _carriedResource = resource;
        _target = resource.transform;
        _state = State.GoingToResource;
    }

    private void Update()
    {
        if (_state == State.Idle || _target == null) return;

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector3 targetPos = _target.position;
        targetPos.y = transform.position.y; 

        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > _stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
            
            Vector3 direction = targetPos - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            OnTargetReached();
        }
    }

    private void OnTargetReached()
    {
        if (_state == State.GoingToResource)
        {   
            _carriedResource.transform.SetParent(transform);
            _carriedResource.transform.localPosition = _carryOffset;

            _target = _homeBase.transform;
            _state = State.Returning;
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