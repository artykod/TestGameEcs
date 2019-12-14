using Unity.Entities;

[System.Serializable]
public struct Gun : IComponentData
{
    public int BulletId;
    public float BulletSpeed;
    public float ShootingFrequency;
}

public class GunComponent : ComponentDataProxy<Gun> { }
