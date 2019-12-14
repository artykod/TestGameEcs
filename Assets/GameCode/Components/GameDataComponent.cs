using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct GameData : ISharedComponentData, IEquatable<GameData>
{
    public float3 FieldSize;
    public GameObject BulletPrefab;
    public GameObject EnemyPrefab;

    public bool Equals(GameData other)
    {
        return BulletPrefab == other.BulletPrefab && EnemyPrefab == other.EnemyPrefab;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class GameDataComponent : SharedComponentDataProxy<GameData> { }
