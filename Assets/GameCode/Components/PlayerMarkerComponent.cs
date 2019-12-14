using Unity.Entities;

public struct PlayerMarker : IComponentData { }

public class PlayerMarkerComponent : ComponentDataProxy<PlayerMarker> { }
