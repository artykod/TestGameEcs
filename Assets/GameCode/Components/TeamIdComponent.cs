using Unity.Entities;

[System.Serializable]
public struct TeamId : IComponentData
{
    public int Value;
}

public class TeamIdComponent : ComponentDataProxy<TeamId> { }