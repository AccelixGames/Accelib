using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Jump : ActionController_Default
    {
        public override string Keyword => "jump";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            playNext?.Invoke(action.value);
        }
    }
}