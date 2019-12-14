using Unity.Entities;

[System.Serializable]
public struct Health : IComponentData
{
    public float Value;
}

public class HealthComponent : ComponentDataProxy<Health> { }
