using UnityEngine;

[CreateAssetMenu(fileName = "ResourceSpawnConfig", menuName = "Resources/Spawn Config")]
public class ResourceSpawnConfig : ScriptableObject
{
    [SerializeField] private ResourceType _type;
    [SerializeField] private Resource _prefab;

    public ResourceType Type => _type;
    public Resource Prefab => _prefab;
}
