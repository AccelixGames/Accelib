using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Module.AccelNovel.Control.Resource;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.View.Ending
{
    public class EndingCreditResourceHandler : MonoBehaviour
    {
        [Header("이미지")]
        [SerializeField] private string key = "thumbnail_";
        [SerializeField, ReadOnly] private EndingCreditImage[] creditImages;

        [Header("오디오")] 
        [SerializeField] private List<string> bgmKeys;
        [SerializeField] private List<string> cgKeys;
        
        
        private HashSet<string> _sprites;
        private HashSet<string> _clips;
        
        private async void OnEnable()
        {
            try
            {
                LoadCreditImages();
            }
            catch (Exception e)
            {
                Debug.Log(e, this);
            }
        }

        public async UniTask LoadResource()
        {
            _sprites = new HashSet<string>();
            _clips = new HashSet<string>();
            
            _sprites = cgKeys.ToHashSet();
            _clips = bgmKeys.ToHashSet();
            
            await ResourceProvider.Instance.Load(_sprites, _clips);
        }
        

        private void LoadCreditImages()
        {
            creditImages = GetComponentsInChildren<EndingCreditImage>();

            if (creditImages.Length <= 0) return;

            var minLength = Math.Min(cgKeys.Count, creditImages.Length);
            var maxLength = Math.Max(cgKeys.Count, creditImages.Length);
            
            for(var i = 0; i < maxLength; i++)
            {
                if (i < minLength)
                {
                    var sprite = ResourceProvider.Instance.GetSprite(cgKeys[i]);
                    var isValid = sprite != null;
                    creditImages[i].gameObject.SetActive(isValid);   
                    if(isValid)
                        creditImages[i].SetSprite(sprite);
                }
                else
                {
                    creditImages[i].gameObject.SetActive(false);   
                }
            }
            
        }

        public void SetResourceKey(List<string> newCgKeys, List<string> newBgmKeys)
        {
            // cgKeys 썸네일용으로 전환
            for (var i = 0; i < newCgKeys.Count; i++)
            {
                if(!newCgKeys[i].StartsWith(key))
                    newCgKeys[i] = key + newCgKeys[i];
            }
            cgKeys = newCgKeys;
            
            bgmKeys = newBgmKeys;
        }
        
        private void OnDestroy()
        {
            // 로드한 에셋 초기화
            ResourceProvider.Instance.ReleaseAll();
        }
    }
}