using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_ChoiceButtonList", menuName = "Maid_Scenario/Dialogue/SO_ChoiceButtonList")]
    public class SO_ChoiceButtonList : ScriptableObject
    {
        [field: Header("일일 제한")] 
        [field: SerializeField] public bool todayLimit { get; private set; }
        [field: SerializeField] public bool todaySeen;
        
        [field:Header("등장 가능한 버튼들")]
        [field:SerializeField] public List<SO_ChoiceButton> buttonList {get; private set;}

        private List<SO_ChoiceButton> firstFilteredButtonList = new();
        private List<SO_ChoiceButton> secondFilteredButtonList = new();
        [field:SerializeField, ReadOnly] private SO_ChoiceButton pickedButton;
        //public SO_ChoiceButton PickedButton => pickedButton;

        public void ResetTodaySeen()
        {
            todaySeen = false;
        }
        

        public SO_ChoiceButton PickedButton()
        {
            if (todayLimit && todaySeen) return null;
            
            firstFilteredButtonList.Clear();
            secondFilteredButtonList.Clear();
            pickedButton = null;

            var priority = -1;
            
            foreach (var button in buttonList)
            {
                if (button.isShow)
                {
                    var hasSeen = button.hasAfterScn && button.nextScenario.hasSeen && !button.nextScenario.continuous;
                    
                    if (!hasSeen)
                        firstFilteredButtonList.Add(button);
                }
            }

            foreach (var filtered in firstFilteredButtonList)
            {
                if (filtered.priority >= priority)
                {
                    if (filtered.priority == priority)
                    {
                        // 같다면 그냥 추가
                        secondFilteredButtonList.Add(filtered);
                    }
                    else
                    {
                        // 크다면 리셋 후 추가
                        secondFilteredButtonList.Clear();
                        secondFilteredButtonList.Add(filtered);
                    }
                        
                    priority = filtered.priority;
                    
                }
            }

            var lastFilterCount = secondFilteredButtonList.Count;
            pickedButton = lastFilterCount > 0 ? secondFilteredButtonList[^1] : null;
            
            return pickedButton;
        }
        
        
    }
}
