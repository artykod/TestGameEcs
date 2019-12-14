using Unity.Entities;

public struct EnemyMarker : IComponentData { }

public class EnemyMarkerComponent : ComponentDataProxy<EnemyMarker> { }
