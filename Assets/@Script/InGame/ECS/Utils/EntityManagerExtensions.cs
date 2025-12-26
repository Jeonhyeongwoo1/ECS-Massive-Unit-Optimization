using Unity.Entities;

public static class EntityManagerExtensions
{
    // 싱글톤이 있으면 값을 업데이트하고, 없으면 새로 만들어서 값을 넣는 함수
    public static void SetOrCreateSingleton<T>(this EntityManager entityManager, T data) 
        where T : unmanaged, IComponentData
    {
        var query = entityManager.CreateEntityQuery(typeof(T));

        if (query.CalculateEntityCount() == 0)
        {
            // 없으면 생성 + 값 설정
            var entity = entityManager.CreateEntity(typeof(T));
            entityManager.SetComponentData(entity, data);
        }
        else
        {
            // 있으면 값만 업데이트
            query.SetSingleton(data);
        }
    }
}