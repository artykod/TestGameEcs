using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct Speed : IComponentData
{
    public float Value;
}

public class SpeedComponent : ComponentDataProxy<Speed> { }
