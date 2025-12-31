using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using UniRx;
using UnityEngine;

public class TimeManager
{
    private CancellationTokenSource _staminaCts;
    private CancellationTokenSource _infinityTicketCts;
    public DateTime LoginTime = DateTime.UtcNow;
    
    public void StaminaTimer()
    {
        if (_staminaCts != null)
        {
            Utils.SafeCancelCancellationTokenSource(ref _staminaCts);
        }
        
        _staminaCts = new CancellationTokenSource();
        // StaminaTimerAsync().Forget();
    }
    
    public void InfiniteTicketTimer()
    {
        if (_infinityTicketCts != null)
        {
            Utils.SafeCancelCancellationTokenSource(ref _infinityTicketCts);
        }

        _infinityTicketCts = new CancellationTokenSource();
        // InfiniteTicketTimerAsync().Forget();
    }

    private async UniTaskVoid StaminaTimerAsync()
    {
        DataManager dataManager = Manager.I.Data;
        GlobalConfigData staminaChargeTimeData = dataManager.GlobalConfigDataDict[GlobalConfigName.StaminaChargeTime];
        GlobalConfigData maxChargedStamina = dataManager.GlobalConfigDataDict[GlobalConfigName.MaxChargedStamina];
        var userModel = ModelFactory.CreateOrGetModel<UserModel>();
        while (true)
        {
            await UniTask.WaitForSeconds(staminaChargeTimeData.Value);
            if (maxChargedStamina.Value <= userModel.Stamina.Value)
            {
                Debug.Log("Stop StaminaTimerAsync");
                break;
            }
            
            Debug.Log($"{maxChargedStamina.Value} / {userModel.Stamina.Value}");

            ChargeType chargeType = ChargeType.STAMINA;
            var response = await Manager.I.Web.SendRequest<StaminaOrTicketResponseData>($"/periodic-charge/{chargeType}", null,
                MethodType.POST.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                break;
            }
            
            userModel.SetUserData(response.data.userData);
            userModel.SetInventory(response.data.inventory);
        }
        
        Utils.SafeCancelCancellationTokenSource(ref _staminaCts);
    }

    private async UniTaskVoid InfiniteTicketTimerAsync()
    {
        DataManager dataManager = Manager.I.Data;
        GlobalConfigData infiniteTicketChargeTime = dataManager.GlobalConfigDataDict[GlobalConfigName.InfiniteTicketChargeTime];
        GlobalConfigData maxChargedInfiniteTicket = dataManager.GlobalConfigDataDict[GlobalConfigName.MaxChargedInfiniteTicket];
        var userModel = ModelFactory.CreateOrGetModel<UserModel>();
        while (true)
        {
            await UniTask.WaitForSeconds(infiniteTicketChargeTime.Value);
            if (maxChargedInfiniteTicket.Value <= userModel.InfinityTicket.Value)
            {
                Debug.Log("Stop InfiniteTicketTimerAsync");
                break;
            }
            
            Debug.Log($"max {maxChargedInfiniteTicket.Value} / {userModel.InfinityTicket.Value}");
            ChargeType chargeType = ChargeType.INFINITY_TICKET;
            var response = await Manager.I.Web.SendRequest<StaminaOrTicketResponseData>($"/periodic-charge/{chargeType}", null,
                MethodType.POST.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                break;
            }
            
            userModel.SetUserData(response.data.userData);
            userModel.SetInventory(response.data.inventory);
        }
        
        Utils.SafeCancelCancellationTokenSource(ref _infinityTicketCts);
    }
}