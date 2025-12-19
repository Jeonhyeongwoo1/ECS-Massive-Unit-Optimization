using System.Collections.Generic;
using System.Linq;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using MewVivor.Util;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class EquipmentMergePopupPresenter : BasePresenter
    {
        private UI_EquipmentMergePopup _popup;

        private Equipment _selectedEquipment;
        private Equipment _mergeEquipment1;
        private Equipment _mergeEquipment2;
        private EquipmentData _resultEquipmentData;
        private EquipmentSortType _equipmentSortType;
        private int _mergeEquipmentSelectCount = 0;
        private List<Equipment> _mergePossibleEquipmentList = new();

        public void OpenEquipmentMergePopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_EquipmentMergePopup>();
            _popup.AddEvent(OnClosePopup, OnMerge, OnAllMerge, OnSortEquipItem, OnReleaseMergeEquipmentItem);
            _equipmentSortType = EquipmentSortType.Level;
            Initialize();
            Refresh(EquipmentGrade.None, EquipmentType.None,0, null);
        }

        private void OnReleaseMergeEquipmentItem(Equipment equipment)
        {
            if (equipment == _selectedEquipment)
            {
                Initialize();
                Refresh(EquipmentGrade.None, EquipmentType.None,0, null);
            }
            else if (equipment == _mergeEquipment1)
            {
                _mergeEquipment1 = null;
                int mergeNeedCount = _selectedEquipment.EquipmentData.MergeNeedCount;
                Refresh(_selectedEquipment.Grade, _selectedEquipment.EquipmentType, mergeNeedCount, _selectedEquipment.EquipmentBaseItemId);
            }
            else if (equipment == _mergeEquipment2)
            {
                _mergeEquipment2 = null;
                int mergeNeedCount = _selectedEquipment.EquipmentData.MergeNeedCount;
                Refresh(_selectedEquipment.Grade, _selectedEquipment.EquipmentType, mergeNeedCount, _selectedEquipment.EquipmentBaseItemId);
            }
        }
        
        private void OnSortEquipItem()
        {
            _equipmentSortType = _equipmentSortType switch
            {
                EquipmentSortType.Grade => EquipmentSortType.Level,
                EquipmentSortType.Level => EquipmentSortType.Grade,
                _ => _equipmentSortType
            };
            
            Initialize();
            Refresh(EquipmentGrade.None, EquipmentType.None,0, null);
        }
        
        private void Refresh(EquipmentGrade equipmentGrade, EquipmentType equipmentType, int mergeCount, int? baseItemId)
        {
            _popup.ReleaseEquipItems();
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            List<Equipment> equipmentList = userModel.AllEquipmentList;
            UIManager uiManager = Manager.I.UI;
            ResourcesManager resourcesManager = Manager.I.Resource;
            List<Equipment> unequipItemList = equipmentList;
            List<Equipment> sortedEquipmentList = null;
            if (_equipmentSortType == EquipmentSortType.Grade)
            {
                sortedEquipmentList = unequipItemList
                    .OrderByDescending(x => x.EquipmentData.Grade)
                    .ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.Level)
                    .ThenBy(x => x.EquipmentData.EquipmentType)
                    .ToList();
            }
            else if (_equipmentSortType == EquipmentSortType.Level)
            {
                sortedEquipmentList = unequipItemList
                    .OrderByDescending(x => x.Level)
                    .ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.EquipmentData.Grade)
                    .ThenBy(x => x.EquipmentData.EquipmentType)
                    .ToList();
            }

            if (sortedEquipmentList != null)
            {
                _mergePossibleEquipmentList.Clear();
                foreach (Equipment equipment in sortedEquipmentList)
                {
                    //Common, Uncommon, Rare
                    if (equipmentType != EquipmentType.None && (equipmentGrade >= EquipmentGrade.Common &&
                                                                     equipmentGrade <= EquipmentGrade.Rare))
                    {
                        if (baseItemId.HasValue && equipment.EquipmentBaseItemId != baseItemId)
                        {
                            continue;
                        }
                    }

                    //Epic~
                    if (equipmentGrade != EquipmentGrade.None && (equipmentGrade >= EquipmentGrade.Epic &&
                                                                  equipmentGrade <= EquipmentGrade.Myth))
                    {
                        if (equipmentGrade != equipment.Grade || equipmentType != equipment.EquipmentType)
                        {
                            continue;
                        }
                    }

                    if (equipment == _selectedEquipment || equipment == _mergeEquipment1 || equipment == _mergeEquipment2)
                    {
                        continue;
                    }

                    if (equipmentType == EquipmentType.None &&
                        !userModel.IsPossibleMergeUnEquipment(equipment, sortedEquipmentList))
                    {
                        continue;
                    }
                    
                    //강화재료에는 레벨 2이상은 안됨
                    if (_selectedEquipment != null)
                    {
                        if (equipment.Level >= 2)
                        {
                            continue;
                        }
                    }

                    var equipItem = uiManager.AddSubElementItem<UI_EquipItem>(_popup.EquipmentGroupTransform);
                    Sprite sprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
                    int level = equipment.Level;
                    Sprite equipTypeSprite =
                        resourcesManager.Load<Sprite>($"{equipment.EquipmentData.EquipmentType}_Icon.sprite");
                    Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
                    equipItem.UpdateUI(sprite, equipTypeSprite, level, gradeSprite, false, equipment.GetGradeCount());
                    bool isSelected = false;
                    if (_selectedEquipment != null)
                    {
                        isSelected = equipment.UID == _selectedEquipment.UID;
                    }

                    equipItem.UpdateMergeEquipmentUI(equipment.IsEquipped(), isSelected);
                    equipItem.AddEvent(()=> OnClickEquipItem(equipment.UID));
                    equipItem.ActivateScaler(true);
                    equipItem.transform.SetAsLastSibling();
                    
                    _mergePossibleEquipmentList.Add(equipment);
                }
            }
            
            _popup.UpdateUI(mergeCount,
                _selectedEquipment,
                _mergeEquipment1,
                _mergeEquipment2,
                _resultEquipmentData, _equipmentSortType);
        }

        private void OnClickEquipItem(string equipmentId)
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            Equipment equipment = userModel.FindEquippedItemOrUnEquippedItem(equipmentId);
            int mergeNeedCount = equipment.EquipmentData.MergeNeedCount;
            if (mergeNeedCount < _mergeEquipmentSelectCount)
            {
                return;
            }
            
            if (_selectedEquipment == null)
            {
                _selectedEquipment = equipment;
            }
            else if (_mergeEquipment1 == null)
            {
                _mergeEquipment1 = equipment;
                _mergeEquipmentSelectCount++;
            }
            else if (_mergeEquipment2 == null)
            {
                if (mergeNeedCount == _mergeEquipmentSelectCount)
                {
                    return;
                }
                
                _mergeEquipment2 = equipment;
                _mergeEquipmentSelectCount++;
            }
            
            //Result
            int itemCode = _selectedEquipment.EquipmentData.MergeSuccessItemCode;
            if (itemCode != 0 && mergeNeedCount == _mergeEquipmentSelectCount)
            {
                EquipmentData equipmentData = Manager.I.Data.EquipmentDataDict[itemCode];
                _resultEquipmentData = equipmentData;
            }

            Refresh(_selectedEquipment.Grade, _selectedEquipment.EquipmentType, mergeNeedCount,
                _selectedEquipment.EquipmentBaseItemId);
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }

            Initialize();
            Manager.I.UI.ClosePopup();
        }

        private async void OnMerge()
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }

            if (_selectedEquipment == null)
            {
                return;
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            EquipmentMergeRequestData requestData = new EquipmentMergeRequestData();
            requestData.materialUserItemIds = new List<string>();
            requestData.materialUserItemIds.Add(_selectedEquipment.UID);
            requestData.materialUserItemIds.Add(_mergeEquipment1.UID);
            if (_mergeEquipment2 != null)
            {
                requestData.materialUserItemIds.Add(_mergeEquipment2.UID);
            }

            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<EquipmentMergeResponseData>("/items/equipments/merge", requestData,
                    MethodType.PATCH.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Initialize();
                Debug.LogError($"Error {response.message}");
                return;
            }
            
            Manager.I.Audio.Play(Sound.SFX, SoundKey.Equipment_MergeSuccess);
            var mergedEquipmentList = new List<Equipment>();
            if (_selectedEquipment != null)
            {
                mergedEquipmentList.Add(_selectedEquipment);
            }

            if (_mergeEquipment1 != null)
            {
                mergedEquipmentList.Add(_mergeEquipment1);
            }

            if (_mergeEquipment2 != null)
            {
                mergedEquipmentList.Add(_mergeEquipment2);
            }
            
            List<UserEquipmentData> userEquipmentDataList = response.data.userItems;
            userModel.MakeEquipment(userEquipmentDataList);
            MergeSuccessPopupPresenter mergeSuccessPopupPresenter =
                PresenterFactory.CreateOrGet<MergeSuccessPopupPresenter>();
            mergeSuccessPopupPresenter.OpenPopup(mergedEquipmentList, new List<UserEquipmentData>() { response.data.resultEquipment });
            
            Initialize();
            Refresh(EquipmentGrade.None, EquipmentType.None, 0, null);
            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            equipmentPopupPresenter.Refresh();
        }

        private async void OnAllMerge()
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<EquipmentMergeAllResponseData>("/items/equipments/merge/all", null,
                    MethodType.PATCH.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Initialize();
                Debug.LogError($"Error {response.message}");
                return;
            }
            
            Manager.I.Audio.Play(Sound.SFX, SoundKey.Equipment_MergeSuccess);
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            List<UserEquipmentData> userEquipmentDataList = response.data.userItems;
            List<UserEquipmentData> userAddedEquipmentDataList = response.data.addedUserItems;
            userModel.MakeEquipment(userEquipmentDataList);

            var mergeAllList = _mergePossibleEquipmentList.Where(x => x.Grade < EquipmentGrade.Epic).ToList();
            MergeSuccessPopupPresenter mergeSuccessPopupPresenter =
                PresenterFactory.CreateOrGet<MergeSuccessPopupPresenter>();
            mergeSuccessPopupPresenter.OpenPopup(mergeAllList, userAddedEquipmentDataList);

            Initialize();
            Refresh(EquipmentGrade.None, EquipmentType.None,0, null);
            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            equipmentPopupPresenter.Refresh();
        }

        private void Initialize()
        {
            _selectedEquipment = null;
            _mergeEquipment1 = null;
            _mergeEquipment2 = null;
            _resultEquipmentData = null;
            _mergeEquipmentSelectCount = 0;
        }
    }
}