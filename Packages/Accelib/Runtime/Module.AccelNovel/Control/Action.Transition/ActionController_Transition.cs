using System;
using Accelib.Data;
using Accelib.Extensions;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using Accelix.Dialogue.DataEnum;
using DG.Tweening;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accelib.Module.AccelNovel.Control.Action.Transition
{
    public class ActionController_Transition : ActionController_Default
    {
        public override string Keyword => "trans";
        public override bool IsPlaying => isChanging;
        public override bool CanSkip => false;
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;

        [SerializeField] private BoolVariable isUILocked;
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private Image image;

        [Header("Debug")]
        [SerializeField] private Color defaultColor = new(29, 30, 35, 255);
        [SerializeField] private Color color = Color.black;
        [SerializeField] private ETransitionType transitionType;
        [SerializeField, Range(0f, 1f), ReadOnly] private float alpha = 0;
        [SerializeField, ReadOnly] private bool isChanging = false;
        
        private Material _mat;
        private Sequence _seq;
        
        private static readonly int ColorAProperty = Shader.PropertyToID("_ColorA");
        private static readonly int ColorBProperty = Shader.PropertyToID("_ColorB");
        private static readonly int FullscreenProperty = Shader.PropertyToID("_IsFullscreen");
        private static readonly int RangeProperty = Shader.PropertyToID("_Range");
        private static readonly int RotationProperty = Shader.PropertyToID("_Rotation");

        private void Awake()
        {
            _mat = Instantiate(image.material);
            image.material = _mat;
        }

        public override void Initialize()
        {
            _mat.SetFloat(RangeProperty, alpha);
            
            alpha = 0f;
            isChanging = false;
            transitionType = ETransitionType.none;
            color = defaultColor;
        }
        
        private void LateUpdate()
        {
            if(isChanging) 
                _mat.SetFloat(RangeProperty, alpha);
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // core
            var key = action.value;
            var show = action.arguments.GetBool("show", true);
            var delay = action.arguments.GetFloat("delay", 0f);
            var afterDelay = action.arguments.GetFloat("afterDelay", 0f);

            if (show) 
                color = action.arguments.GetColor("color", defaultColor);
            
            // 변경 시작
            isChanging = true;
            isUILocked.Value = true;
            
            // anim
            if (!Enum.TryParse(key, true, out transitionType))
                transitionType = ETransitionType.fade;
            
            if (show) 
                DoFadeIn();
            else 
                DoFadeOut();

            _seq.PrependInterval(delay);
            _seq.AppendInterval(afterDelay);
            _seq.onComplete += () =>
            {
                isUILocked.Value = false;
                playNext?.Invoke(null);
            };
        }

        public override void SkipAction(bool isFastForward = false)
        {
            // if(isFastForward)
                // _seq.Complete(true);
        }

        public Tweener DoFadeIn()
        {
            switch (transitionType)
            {
                case ETransitionType.fade_horizontal:
                    return Internal_DoFade(true, Direction.Left, false);
                case ETransitionType.fade_vertical:
                    return Internal_DoFade(true, Direction.Up, false);
                case ETransitionType.fade:
                    return Internal_DoFade(true, Direction.Left, true);
                case ETransitionType.none:
                default:
                    return null;
            }
        }

        public DG.Tweening.Tween DoFadeOut()
        {
            switch (transitionType)
            {
                case ETransitionType.fade_horizontal:
                    return Internal_DoFade(false, Direction.Right, false);
                case ETransitionType.fade_vertical:
                    return Internal_DoFade(false, Direction.Down, false);
                case ETransitionType.fade:
                    return Internal_DoFade(false, Direction.None, true);
                case ETransitionType.none:
                default:
                    return null;
            }
        }

        private Tweener Internal_DoFade(bool enable, Direction dir, bool fullscreen)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            var tween = DOTween.To(() => alpha, x => alpha = x, enable ? 1f : 0f, config.duration);
            tween.SetEase(enable ? config.easeA : config.easeB);
            tween.SetLink(gameObject);
            
            tween.OnPlay(() =>
            {
                var cA = color; cA.a = 0f;
                var cB = color; cB.a = 1f;
                _mat.SetColor(ColorAProperty, cA);
                _mat.SetColor(ColorBProperty, cB);
                _mat.SetInt(FullscreenProperty, fullscreen ? 1 : 0);
                if(dir != Direction.None)
                    _mat.SetFloat(RotationProperty, dir.ToAngle());
            });
            
            tween.OnComplete(() =>
            {
                isChanging = false;
                _mat.SetFloat(RangeProperty, alpha);
            });

            _seq.Append(tween);
            
            return tween;
        }
    }
}