using Unity.Entities;

[System.Serializable]
public struct Damage : IComponentData
{
    public float Value;
}

public class DamageComponent : ComponentDataProxy<Damage> { }
