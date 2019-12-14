using Unity.Entities;

[System.Serializable]
public struct SphereCollider : IComponentData
{
    public float Radius;
}

public class SphereColliderComponent : ComponentDataProxy<SphereCollider> { }