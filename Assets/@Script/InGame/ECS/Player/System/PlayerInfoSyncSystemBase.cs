using MewVivor;
using MewVivor.InGame.Controller;
using Unity.Entities;

public partial class PlayerInfoSyncSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        PlayerController player = Manager.I.Object.Player;
        if (player == null)
        {
            return;
        }
        
        EntityManager.SetOrCreateSingleton(new PlayerInfoComponent
        {
            Position = player.Position,
            Radius = player.Radius,
            CriticalPercent = player.CriticalPercent.Value,
            CriticalDamagePercent = player.CriticalDamagePercent.Value,
            Atk = player.Atk.Value
        });
    }
}