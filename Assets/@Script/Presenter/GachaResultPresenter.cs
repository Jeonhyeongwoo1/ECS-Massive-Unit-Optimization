using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class GachaResultPresenter : BasePresenter
    {
        private UserModel _model;
        public void Initialize(UserModel model)
        {
            _model = model;
        }
        
    }
}