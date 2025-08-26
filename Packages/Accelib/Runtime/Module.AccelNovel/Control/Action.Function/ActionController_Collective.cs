using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.Collective.SO;
using Accelib.Module.Audio.Data._Base;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Collective : ActionController_Default
    {
        public override string Keyword => "collective";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;

        [Header("Collective")]
        [SerializeField] private SerializedDictionary<string, CollectiveSO_MiniGame> collectiveDict;

        [SerializeField, TextArea] private string msg;
        [SerializeField] private AudioRefBase onCollectedSfx;

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var key = action.value;
            var collective = collectiveDict.GetValueOrDefault(key);
            
            // 컬렉션을 잘 가져왔다면,
            if (collective)
            {
                // 컬렉트 해보고, 성공했다면,
                if (collective.OnCollect())
                {
                    // 해금 진행
                    var collectiveName = collective.Title;
                    onCollectedSfx?.PlayOneShot();
                    // ToastSystem.Open(msg.Replace("{0}", collectiveName));
                }
                else
                    Deb.Log($"{key} 는 이미 해금되었습니다.", this);
            }
            else
                Deb.LogError($"{key} 는 등록되지 않은 컬렉션입니다. 해금할 수 없습니다.", this);

            playNext?.Invoke(null);
        }
    }
}