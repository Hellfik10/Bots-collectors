using System.Collections;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private ResourceTracker _tracker;
    [SerializeField] private float _scanRadius = 45f;
    [SerializeField] private float _scanInterval = 0.5f;

    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (enabled)
        {
            Scan();
            yield return new WaitForSeconds(_scanInterval);
        }
    }

    private void Scan()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Resource resource))
            {
                _tracker.RegisterResource(resource);
            }
        }
    }
}
