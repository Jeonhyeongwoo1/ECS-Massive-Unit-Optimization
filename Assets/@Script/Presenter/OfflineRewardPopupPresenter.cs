using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class OfflineRewardPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private TimeDataModel _timeDataModel;
        private UI_OfflineRewardPopup _popup;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            _timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
        }

    }
}