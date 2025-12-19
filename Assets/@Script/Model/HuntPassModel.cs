using MewVivor.Data.Server;
using MewVivor.Interface;
using UniRx;

namespace MewVivor.Model
{
    public class HuntPassModel : IModel
    {
        public ReactiveProperty<bool> IsPossibleGetReward = new ReactiveProperty<bool>();
        public int RemainBoostCount => _huntPassData.remainBoostCount;
        public int DailyBoostChargeCount => _huntPassData.dailyBoostChargeCount; //내가 본 광고 횟수
        public HuntPassData HuntPassData => _huntPassData;
        
        private HuntPassData _huntPassData;

        public void SetHuntPassData(HuntPassData huntPassData)
        {
            _huntPassData = huntPassData;

            if (
                (_huntPassData.isPremium && _huntPassData.premiumClaimableLevel.HasValue &&
                 _huntPassData.premiumClaimableLevel.Value > _huntPassData.lastReceivedPremiumLevel)
                || (_huntPassData.normalClaimableLevel.HasValue &&
                    _huntPassData.normalClaimableLevel.Value > _huntPassData.lastReceivedLevel))
            {
                IsPossibleGetReward.Value = true;
            }
            else
            {
                IsPossibleGetReward.Value = false;
            }
        }

        public void Reset()
        {
            _huntPassData = null;
        }
    }
}