using System;
using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _stopDistance = 0.2f;

    private Transform _target;
    private Coroutine _movementCoroutine;

    public event Action DestinationReached;

    private void OnEnable()
    {
        if (_target != null)
        {
            _movementCoroutine = StartCoroutine(MoveToTarget());
        }
    }

    private void OnDisable()
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
    }

    public void MoveTo(Transform target)
    {
        _target = target;
        if (_movementCoroutine == null && isActiveAndEnabled)
        {
            _movementCoroutine = StartCoroutine(MoveToTarget());
        }
    }

    public void StopMoving()
    {
        _target = null;
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
    }

    private IEnumerator MoveToTarget()
    {
        yield return null;

        while (_target != null)
        {
            Vector3 targetPosition = _target.position;
            targetPosition.y = transform.position.y;

            if (transform.position.IsEnoughClose(targetPosition, _stopDistance))
            {
                _target = null;
                _movementCoroutine = null;
                DestinationReached?.Invoke();
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            yield return null;
        }

        _movementCoroutine = null;
    }
}
