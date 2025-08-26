using Accelib.Logging;
using Accelib.Module.SaveLoad.SaveDataHolder;

namespace Accelix.Singleton.SaveLoad.Statistics
{
    public class SaveDataHolder_Stats : SaveDataHolderBaseT<SaveData_Stats>
    {
        public int GetReplayCount() => data.replayCount;
        
        public void ClearReplayCount()
        {
            data.replayCount = 0;
            SetDirty();
            
            Deb.Log($"클리어 횟수 초기화! 현재:{data.replayCount}");
        }

        public void AddReplayCount()
        {
            data.replayCount += 1;
            SetDirty();
            
            Deb.Log($"클리어 횟수 증가! 현재:{data.replayCount}");
        }
    }
}