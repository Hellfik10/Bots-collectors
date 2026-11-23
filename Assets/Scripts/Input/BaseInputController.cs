using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class BaseInputController : MonoBehaviour
{
    private Camera _camera;
    private BaseExpansionController _selectedBase;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
        {
            return;
        }

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        BaseExpansionController clickedBase = FindClickedBase(ray);
        if (clickedBase != null)
        {
            _selectedBase = clickedBase;
            return;
        }

        if (_selectedBase != null)
        {
            _selectedBase.TrySetExpansionFlag(ray);
        }
    }

    private static BaseExpansionController FindClickedBase(Ray ray)
    {
        BaseExpansionController clickedBase = null;
        float closestBaseHit = Mathf.Infinity;

        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            BaseExpansionController hitBase = hit.collider.GetComponentInParent<BaseExpansionController>();
            if (hitBase != null && hit.distance < closestBaseHit)
            {
                clickedBase = hitBase;
                closestBaseHit = hit.distance;
            }
        }

        return clickedBase;
    }
}
