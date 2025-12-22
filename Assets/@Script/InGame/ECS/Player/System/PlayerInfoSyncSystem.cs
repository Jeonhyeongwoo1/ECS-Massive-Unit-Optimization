using MewVivor;
using MewVivor.InGame.Controller;
using Unity.Entities;

public partial class PlayerInfoSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        PlayerController player = Manager.I.Object.Player;
        if (player == null)
        {
            return;
        }

        if (SystemAPI.HasSingleton<PlayerInfoComponent>())
        {
            SystemAPI.SetSingleton(new PlayerInfoComponent()
            {
                Position = player.Position
            });
        }
        else
        {
            Entity entity= EntityManager.CreateEntity(typeof(PlayerInfoComponent));
            SystemAPI.SetComponent(entity, new PlayerInfoComponent()
            {
                Position = player.Position,
                Radius = player.Radius
            });
        }
    }
}