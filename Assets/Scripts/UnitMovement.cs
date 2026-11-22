using System;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _stopDistance = 0.2f;

    private Transform _target;

    public event Action OnDestinationReached;

    public void MoveTo(Transform target)
    {
        _target = target;
    }

    public void StopMoving()
    {
        _target = null;
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position;
        targetPosition.y = transform.position.y;

        if (transform.position.IsEnoughClose(targetPosition, _stopDistance))
        {
            _target = null;
            OnDestinationReached?.Invoke();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
