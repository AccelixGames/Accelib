using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.Collective.Achievement;
using Accelix.Accelib.AccelNovel.Model;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Achievement : ActionController_Default
    {
        public override string Keyword => "achievement";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;
        
        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var key = action.value;
            var result = AchievementSingleton.Instance.CheckAndAchieve(key);
            
            // 애널리틱스
            // if(result)
            //     AnalyticsHandler.OnUnlock(Novel.ReplayCount, key, AnalyticsHandler.UnlockType.Achievement);
            
            playNext?.Invoke(null);
        }
    }
}