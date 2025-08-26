using System.Collections.Generic;
using Accelib.Module.SaveLoad.SaveDataHolder;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Implements.SaveLoad.MiniGame2
{
    public class SaveDataHolder_MiniGame2 : SaveDataHolderBaseT<SaveData_MiniGame2>
    {
        public MaxScoreData GetMaxScoreData(string key)
        {
            return data.saveData.GetValueOrDefault(key);
        }
        
        public void SetMaxScoreData(string key, int score, int combo)
        {
            if(string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) return;
            
            if (data.saveData.TryGetValue(key, out var scoreData))
            {
                // 기존에 저장된 정보가 있다면 최대값 저장
                scoreData.maxScore = Mathf.Max(scoreData.maxScore, score);
                scoreData.maxCombo = Mathf.Max(scoreData.maxCombo, combo);
            }
            else
            {
                // 저장된 정보가 없다면 추가
                var newScoreData = new MaxScoreData { maxCombo = combo, maxScore = score };
                data.saveData.Add(key, newScoreData);
            }
            
            SetDirty();
        }
    }
}