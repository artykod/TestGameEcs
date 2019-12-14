using Unity.Entities;

public struct DestroyOutOfBound : IComponentData { }

public class DestroyOutOfBoundComponent : ComponentDataProxy<DestroyOutOfBound> { }
