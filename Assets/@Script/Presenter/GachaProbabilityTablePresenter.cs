using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class GachaProbabilityTablePresenter : BasePresenter
    {
        private UI_GachaProbabilityTablePopup _popup;

        public void OpenPopup(GachaProbabilityTableType probabilityTableType)
        {
            _popup = Manager.I.UI.OpenPopup<UI_GachaProbabilityTablePopup>();

            Refresh(probabilityTableType);
        }

        private void Refresh(GachaProbabilityTableType probabilityTableType)
        {
            _popup.Release();
            switch (probabilityTableType)
            {
                case GachaProbabilityTableType.EquipmentNormal:
                {
                    // 1. 표시할 등급 목록 정의
                    var targetGrades = new HashSet<EquipmentGrade>
                    {
                        EquipmentGrade.Common, EquipmentGrade.Uncommon, EquipmentGrade.Rare, EquipmentGrade.Epic
                    };

                    // 2. 해당 등급의 아이템들을 'Grade'별로 그룹화
                    var groupedByGrade = Manager.I.Data.EquipmentDataDict.Values
                        .Where(x => targetGrades.Contains(x.Grade) && !string.IsNullOrEmpty(x.DropProbId))
                        .GroupBy(x => x.Grade)
                        .OrderBy(g => g.Key); // 등급 순서로 정렬

                    // 3. 각 등급 그룹을 순회하며 개별 확률 계산 및 UI 업데이트
                    foreach (var gradeGroup in groupedByGrade)
                    {
                        int itemCountInGrade = gradeGroup.Count();
                        if (itemCountInGrade == 0) continue;

                        // 그룹의 첫 아이템을 기준으로 총 확률 가져오기 (동일 등급은 확률이 같다고 가정)
                        string dropId = gradeGroup.First().DropProbId;
                        var dropData = Manager.I.Data.EquipmentDropDataDict[dropId];
                        float totalGradeProb = dropData.SilverKeyDropProb;

                        // 4. 개별 아이템의 실제 드랍 확률 계산
                        float individualProb = totalGradeProb / itemCountInGrade;

                        // 5. 그룹 내 모든 아이템에 대해 계산된 개별 확률로 UI 업데이트
                        foreach (var equipmentData in gradeGroup)
                        {
                            var element =
                                Manager.I.UI.AddSubElementItem<UI_GachaProbabilityTableElement>(
                                    _popup.GachaElementParent);
                            string equipmentName = Manager.I.Data.LocalizationDataDict[equipmentData.NameTextID]
                                .GetValueByLanguage();
                            Color color = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentData.Grade);

                            // 소수점 4자리까지 표시 (예: 0.1234%)
                            element.UpdateUI(equipmentName, $"{individualProb * 100:F4}%", color);
                        }
                    }

                    break;
                }

                case GachaProbabilityTableType.EquipmentRare:
                {
                    var targetGrades = new HashSet<EquipmentGrade>
                    {
                        EquipmentGrade.Uncommon, EquipmentGrade.Rare, EquipmentGrade.Epic,
                        EquipmentGrade.Epic1, EquipmentGrade.Epic2, EquipmentGrade.Legendary
                    };

                    var groupedByGrade = Manager.I.Data.EquipmentDataDict.Values
                        .Where(x => targetGrades.Contains(x.Grade) && !string.IsNullOrEmpty(x.DropProbId))
                        .GroupBy(x => x.Grade)
                        .OrderBy(g => g.Key);

                    foreach (var gradeGroup in groupedByGrade)
                    {
                        int itemCountInGrade = gradeGroup.Count();
                        if (itemCountInGrade == 0) continue;

                        string dropId = gradeGroup.First().DropProbId;
                        var dropData = Manager.I.Data.EquipmentDropDataDict[dropId];
                        float totalGradeProb = dropData.GoldenKeyDropProb;

                        float individualProb = totalGradeProb / itemCountInGrade;

                        foreach (var equipmentData in gradeGroup)
                        {
                            var element =
                                Manager.I.UI.AddSubElementItem<UI_GachaProbabilityTableElement>(
                                    _popup.GachaElementParent);
                            string equipmentName = Manager.I.Data.LocalizationDataDict[equipmentData.NameTextID]
                                .GetValueByLanguage();
                            Color color = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentData.Grade);
                            element.UpdateUI(equipmentName, $"{individualProb * 100:F4}%", color);
                        }
                    }

                    break;
                }

                case GachaProbabilityTableType.Scroll:
                    float ratio = (float)100 / 6;
                    for (int i = (int)MaterialType.WeaponScroll; i <= (int)MaterialType.BootsScroll; i++)
                    {
                        var element =
                            Manager.I.UI.AddSubElementItem<UI_GachaProbabilityTableElement>(_popup.GachaElementParent);
                        Color color = Const.EquipmentUIColors.GetEquipmentGradeColor(EquipmentGrade.Common);
                        string name = Manager.I.Data.LocalizationDataDict[((MaterialType)i).ToString()]
                            .GetValueByLanguage();
                        element.UpdateUI(name, $"{ratio:F2}%", color); // 소수점 2자리까지 표시
                    }

                    break;
            }
        }
    }
}