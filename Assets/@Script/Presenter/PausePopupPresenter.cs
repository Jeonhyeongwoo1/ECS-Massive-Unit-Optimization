using System.Collections.Generic;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Popup;
using MewVivor.InGame.Skill;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class PausePopupPresenter : BasePresenter
    {
        private UI_PausePopup _popup;
        private PlayerModel _playerModel;
        
        public void Initialize(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        public void OpenPausePopup()
        {
            Time.timeScale = 0;
            _popup = Manager.I.UI.OpenPopup<UI_PausePopup>();
            _popup.AddEvent(OnResumeGame, OnExitGameButton, OnActivateSound, OnShowStatisticsPopup);

            List<BaseAttackSkill> attackSkillList = Manager.I.Object.Player.SkillBook.ActivateAttackSkillList;
            List<BasePassiveSkill> passiveSkillList = Manager.I.Object.Player.SkillBook.ActivatePassiveSkillList;
            
            string level = $"LV.{_playerModel.CurrentLevel.Value}";
            float ratio = _playerModel.CurrentExpRatio.Value;
            _popup.UpdateUI(attackSkillList, passiveSkillList, level, ratio);
        }
        
        private void OnShowStatisticsPopup()
        {
            var presenter = PresenterFactory.CreateOrGet<StatisticsPopupPresenter>();
            presenter.OpenStatisticsPopup();
        }

        private void OnActivateSound(bool isClickBGMButton)
        {
            if (isClickBGMButton)
            {
                Manager.I.IsOnBGM = !Manager.I.IsOnBGM;
            }
            else
            {
                Manager.I.IsOnSfx = !Manager.I.IsOnSfx;
            }
            
            _popup.UpdateSoundUI(Manager.I.IsOnBGM, Manager.I.IsOnSfx);
        }

        private void OnResumeGame()
        {
            Time.timeScale = 1;
            Manager.I.UI.ClosePopup();
        }

        private void OnExitGameButton()
        {
            var exitGamePresenter = PresenterFactory.CreateOrGet<ExitGamePresenter>();
            exitGamePresenter.OpenExitGamePopup();
        }
    }
}