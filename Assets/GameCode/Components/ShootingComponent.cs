using Unity.Entities;

public struct Shooting : IComponentData
{
    public Gun Gun;
    public float Timer;
}