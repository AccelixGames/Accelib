using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Parameter;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Branch : ActionController_Default
    {
        public override string Keyword => "branch";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;

        [Header("Branch")]
        [SerializeField] private ParameterController paramController;
        
        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // key
            var varKey = action.value;
            var value = action.arguments.GetInt("value", 0);
            var onBig = action.arguments.GetString("onBig", null);
            var onSmall = action.arguments.GetString("onSmall", null);

            var currValue = paramController.GetValue(varKey);
            if (!currValue.HasValue)
            {
                Deb.LogError($"변수명 {varKey} 를 찾을 수 없습니다.", this);
                playNext?.Invoke(null);
            }
            
            // 분기별로 점프
            var isBig = currValue.GetValueOrDefault(0) >= value;
            Deb.Log($"Branch: {varKey}({currValue.GetValueOrDefault(-1)}) >= {value} ? {onBig} : {onSmall} ==> {isBig}");
            if(isBig)
                playNext?.Invoke(onBig);
            else
                playNext?.Invoke(onSmall);
        }
    }
}