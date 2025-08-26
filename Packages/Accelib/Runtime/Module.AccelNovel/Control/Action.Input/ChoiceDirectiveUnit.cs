using Accelib.Tween;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Action.Input
{
    public class ChoiceDirectiveUnit : MonoBehaviour
    {
        private RectTransform _rt;
        
        [SerializeField] private TMP_Text directiveTMP;
        [SerializeField] private RectTransform startRect;
        [SerializeField] private RectTransform endRect;

        public DG.Tweening.Tween Open(string text, DefaultTweenValue tweenValue)
        {
            // 지시문 적용
            directiveTMP.text = text;
            
            // 시작 위치로 이동
            _rt = GetComponent<RectTransform>();
            _rt.anchoredPosition = startRect.anchoredPosition;
            
            gameObject.SetActive(true);

            // 트윈 진행
            return _rt.DOAnchorPosY(endRect.anchoredPosition.y, tweenValue.duration)
                .SetEase(tweenValue.ease)
                .SetDelay(tweenValue.delay)
                .SetLink(gameObject);
        }
        
#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void OpenTest() => Open("테스트 문장입니다.", new DefaultTweenValue(0.5f, 0.3f, Ease.OutSine));
#endif
    }
}