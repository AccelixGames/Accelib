using Accelib.Data;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accelib.Module.AccelNovel.View.Ending
{
    public class Ending_Card : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image thumbnail;
        [SerializeField] private TMP_Text comment;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private EasePairTweenConfig fadeTween;
        
        [Header("종료 딜레이")]
        [SerializeField] private float delayTime = 3;

        [Header("종료 이벤트")]
        [SerializeField] private UnityEvent onEnd;
        
        public void Open(EndingData data)
        {
            if(data?.collective?.Image == null)
                thumbnail.gameObject.SetActive(false);
            
            thumbnail.sprite = data?.collective?.Image;
            comment.text = data?.collective?.Msg;

            gameObject.SetActive(true);

            Close().Forget();
        }

        private async UniTaskVoid Close()
        {
            await UniTask.WaitForSeconds(delayTime);

            onEnd?.Invoke();
        }
    }
}