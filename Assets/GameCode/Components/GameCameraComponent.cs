using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct GameCamera : IComponentData
{
    public Entity Target;
    public float3 EulerRotation;
}

public class GameCameraComponent : ComponentDataProxy<GameCamera> { }
