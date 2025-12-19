using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;

namespace MewVivor.Presenter
{
    public class ProfilePopupPresenter : BasePresenter
    {
        private UI_ProfilePopup _popup;

        public void OpenPopup()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            string nickName = userModel.UserData.name;
            _popup = Manager.I.UI.OpenPopup<UI_ProfilePopup>();
            _popup.AddEvent(ClosePopup, OnChangeUserNickname);

            bool isPossibleChangeNickNameByJewel = userModel.Jewel.Value >= Const.Nickname_change_price;
            bool isFree = !userModel.UserData.isNameChanged;
            _popup.UpdateUI(nickName, Const.Nickname_change_price.ToString(), isPossibleChangeNickNameByJewel, isFree);
        }

        private void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        private async void OnChangeUserNickname(string name)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }

            bool isPossibleChangeNickNameByJewel = false;
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            if (userModel.UserData.isNameChanged)
            {
                isPossibleChangeNickNameByJewel = userModel.Jewel.Value >= Const.Nickname_change_price;
                if (!isPossibleChangeNickNameByJewel)
                {
                    string message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Jewel"].GetValueByLanguage();
                    Manager.I.UI.OpenSystemPopup(message);
                    return;
                }
            }

            if (BannedWords.IsCanUseNickName(name))
            {
                string message = Manager.I.Data.LocalizationDataDict["Name_NOT_Available"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }
            
            ChangeUserNameRequestData requestData = new ChangeUserNameRequestData
            {
                name = name
            };

            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<ChangeUserNameResponseData>("/user/name", requestData,
                    MethodType.PATCH.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                string message = "";
                switch(response.message)
                {
                    case ServerMessageType.NAME_ALREADY_EXISTS:
                        message = Manager.I.Data.LocalizationDataDict["NAME_ALREADY_EXISTS"].GetValueByLanguage();
                        break;
                    case ServerMessageType.NAME_LENGTH_INVALID:
                        message = Manager.I.Data.LocalizationDataDict["NAME_LENGTH_INVALID"].GetValueByLanguage();
                        break;
                    case ServerMessageType.NAME_SPECIAL_CHARACTER_INVALID:
                        message = Manager.I.Data.LocalizationDataDict["NAME_SPECIAL_CHARACTER_INVALID"].GetValueByLanguage();
                        break;
                    case ServerMessageType.NAME_INCLUDES_PROFANE_WORD:
                        message = Manager.I.Data.LocalizationDataDict["NAME_INCLUDES_PROFANE_WORD"].GetValueByLanguage();
                        break;
                }

                Manager.I.UI.OpenSystemPopup(message);
                return;
            }
            
            userModel.SetUserData(response.data.userData);
            userModel.SetInventory(response.data.inventory);
            isPossibleChangeNickNameByJewel = userModel.Jewel.Value >= Const.Nickname_change_price;
            bool isFree = !userModel.UserData.isNameChanged;
            _popup.UpdateUI(name, Const.Nickname_change_price.ToString(), isPossibleChangeNickNameByJewel, isFree);
            ClosePopup();
            LobbyPresenter lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            lobbyPresenter.UpdateUI();
        }
    }
}