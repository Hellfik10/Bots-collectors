using UnityEngine;

public class BaseExpansionController : MonoBehaviour
{
    [SerializeField] private Collider _mapCollider;

    private BaseUnitManager _unitManager;

    private void Awake()
    {
        _unitManager = GetComponent<BaseUnitManager>();
    }

    public bool TrySetExpansionFlag(Ray ray)
    {
        if (_mapCollider == null)
        {
            Debug.LogError("[BaseExpansionController] Не задан коллайдер карты для размещения флага", this);
            return false;
        }

        if (_mapCollider.Raycast(ray, out RaycastHit mapHit, Mathf.Infinity) == false)
        {
            return false;
        }

        _unitManager.SetExpansionFlag(mapHit.point);
        return true;
    }
}
