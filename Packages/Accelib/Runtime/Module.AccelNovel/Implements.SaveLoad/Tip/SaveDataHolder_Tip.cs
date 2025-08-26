using System.Collections.Generic;
using Accelib.Module.SaveLoad.SaveDataHolder;

namespace Accelix.Singleton.SaveLoad.Tip
{
    public class SaveDataHolder_Tip : SaveDataHolderBaseT<SaveData_Tip>
    {
        public int GetMiniGameMaxLevel()
        {
            return data.miniGameLevelMax;
        }

        public void OnMiniGameLevelCleared(int currLevel)
        {
            if (currLevel == data.miniGameLevelMax)
            {
                data.miniGameLevelMax += 1;
                SetDirty();
            }
        }
        
        public bool CheckNewTipAndUpdate(string tip)
        {
            data.tipSaves ??= new List<string>();
            
            if (data.tipSaves.Contains(tip)) return false;
            
            data.tipSaves.Add(tip);
            SetDirty();
            return true;
        }

        public void ClearTip(string tip)
        {
            data.tipSaves ??= new List<string>();

            if (data.tipSaves.Contains(tip))
            {
                data.tipSaves.Remove(tip);
                SetDirty();
            }
        }

        public void ClearAllTips()
        {
            if (data.tipSaves is not { Count: > 0 }) return;
            
            data.tipSaves = new List<string>();
            SetDirty();
        }
    }
}