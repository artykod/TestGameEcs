using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[RequireComponent(typeof(GameObjectEntity))]
public class CameraTargetHelper : MonoBehaviour
{
    public GameObjectEntity Target;

    private void Start()
    {
        var cameraEntity = GetComponent<GameObjectEntity>();
        var gameCamera = cameraEntity.EntityManager.GetComponentData<GameCamera>(cameraEntity.Entity);
        gameCamera.Target = Target.Entity;
        cameraEntity.EntityManager.SetComponentData(cameraEntity.Entity, gameCamera);
    }

    private void LateUpdate()
    {
        var cameraEntity = GetComponent<GameObjectEntity>();
        var em = cameraEntity.EntityManager;

        transform.position = em.GetComponentData<Translation>(cameraEntity.Entity).Value;
        transform.rotation = em.GetComponentData<Rotation>(cameraEntity.Entity).Value;
    }
}
