using System.Collections.Generic;
using Accelib.Module.AccelNovel.Control.Action.Input;
using Accelib.Module.AccelNovel.Model.Collective.Achievement;
using Accelib.Module.AccelNovel.Model.Collective.Enum;
using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using Accelib.Module.SaveLoad.SaveDataHolder;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Implements.SaveLoad.Collective
{
    public class SaveDataHolder_Collective : SaveDataHolderBaseT<SaveData_Collective>
    {
        [SerializeField] private List<CollectiveAchievement> collectiveAchievements;
        
        [Header("All choice achievements")]
        [SerializeField] private SO_Achievement choiceAchievement;
        [SerializeField] private int choiceCount;
        
        // 컬렉션의 상태를 가져온다.
        public CollectiveState GetState(string key) => 
            string.IsNullOrEmpty(key) ? CollectiveState.Locked : data?.saveData?.GetValueOrDefault(key, CollectiveState.Locked) ?? CollectiveState.Locked;
        
        // 컬렉션의 상태를 가져온다.
        public CollectiveState GetState(CollectiveSO_Base so) => GetState(so?.AssetKey);
        
        // 컬렉션의 상태를 설정한다.
        public void SetState(CollectiveSO_Base so, CollectiveState state) => SetState(so?.AssetKey, state);
        
        // 컬렉션의 상태를 설정한다.
        public bool SetState(string key, CollectiveState state)
        {
            // 키가 없으면 종료
            if(string.IsNullOrEmpty(key)) return false;
            
            // 같다면, 종료
            if(data.saveData.TryGetValue(key, out var dataState))
                if(dataState == state)
                    return false;
            
            // 다르다면, 설정
            data.saveData[key] = state;
            SetDirty();
            
            // 도전과제 해금 시도
            foreach (var collectiveAchievement in collectiveAchievements) 
                collectiveAchievement.CheckAndAchieve(in data.saveData);
            
            // 모든 선택지 도전과제 해금 시도
            var currCount = 0;
            foreach (var (saveKey, collectiveState) in data.saveData)
                if(collectiveState is CollectiveState.Unlocked or CollectiveState.UnlockedAsNew)
                    if (saveKey.Contains(ActionController_Choice.CollectivePrefix))
                        currCount += 1;
            if (currCount >= choiceCount)
                choiceAchievement?.CheckAndAchieve();
            
            return true;
        }
        
        public bool TryUnlock(string key, bool openToast, string unlockedText)
        {
            if (GetState(key) == CollectiveState.Locked)
            {
                var result = SetState(key, CollectiveState.UnlockedAsNew);
                // if(result && openToast)
                //     ToastSystem.Open(unlockedText);
                
                return result;
            }
            
            return false;
        }
    }
}