using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _type;

    public ResourceType Type => _type;

    public void Initialize(ResourceType type)
    {
        _type = type;
    }
}
