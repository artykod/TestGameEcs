using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct GameCameraListener : IComponentData
{
    public quaternion Rotation;
}

public class GameCameraListenerComponent : ComponentDataProxy<GameCameraListener> { }
