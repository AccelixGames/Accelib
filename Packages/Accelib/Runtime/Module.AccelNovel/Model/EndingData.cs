using Accelib.Module.AccelNovel.Model.Collective.SO;
using Accelix.Accelib.AccelNovel.Model.Enum;

namespace Accelib.Module.AccelNovel.Model
{       
    [System.Serializable]
    public class EndingData
    {
        public bool replayCountUp = true;
        public EEndingType endingType;
        public string bgmKey;
        public CollectiveSO_Ending collective;
    }
}