using System.Collections.Generic;
using Unity.Entities;

public class GameSystem : ComponentSystem
{
    public GameData Data
    {
        get;
        private set;
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        var data = new List<GameData>(2);
        EntityManager.GetAllUniqueSharedComponentData(data);
        Data = data[1];
    }

    protected override void OnUpdate()
    {
    }
}