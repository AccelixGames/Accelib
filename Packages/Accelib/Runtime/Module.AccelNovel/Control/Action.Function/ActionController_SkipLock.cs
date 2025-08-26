using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_SkipLock : ActionControllerT<ActionController_SkipLock.Data>
    {
        [System.Serializable]
        public class Data
        {
            public bool isLocked;
        }
        
        public override string Keyword => "skipLock";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;

        [Header("SkipLock")]
        [SerializeField] private bool isPlaying;
        [SerializeField] private BoolVariable isSkipLocked;
        
        private Sequence _seq;

        public override void Initialize()
        {
            isPlaying = false;
            _seq?.Kill();
            _seq = null;
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var key = action.value;
            if (!bool.TryParse(key, out var isLocked))
                isLocked = false;
            var delay = action.arguments.GetFloat("delay", 0f);

            isPlaying = true;
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.AppendInterval(delay);
            _seq.AppendCallback(() =>
            {
                isPlaying = false;
                isSkipLocked.Value = isLocked;
                playNext?.Invoke(null);
            });
        }
        
        protected override void Internal_FromJson(Data data)
        {
            isSkipLocked.Value = data.isLocked;
        }

        protected override Data Internal_GetData()
        {
            return new Data
            {
                isLocked = isSkipLocked.Value
            };
        }
    }
}