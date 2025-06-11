#if ACCELIB_AIT
using System;
using Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen;
using Accelib.AccelixWeb.Module.Advertisement.Model;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator
{
    public class AdsEmulator : MonoBehaviour
    {
        private enum State {NotLoaded = 0, Loading, Loaded, Shown}
        [Flags] 
        public enum EventFlag
        {
            Loaded = 1 << 0,
            Show = 1 << 1,
            Impression = 1 << 2,
            UserEarnedReward = 1 << 3,
            Dismissed = 1 << 4,
            Requested = 1 << 5,
            Clicked = 1 << 6
        }

        [Header("옵션")]
        [SerializeField, Range(0f, 10f)] private float delayTime = 0.2f;
        [SerializeField] private bool loadSuccess;
        [SerializeField] private bool showSuccess;
        [field: SerializeField, EnumFlags] public EventFlag EventToInvoke { get; private set; } = (EventFlag)int.MaxValue;

        [Header("UI")]
        [SerializeField] private AdsEmulator_Interstitial interstitialEmulator;
        [SerializeField] private AdsEmulator_Rewarded rewardedEmulator;

        [Header("상태")]
        [SerializeField, ReadOnly] private State currState = State.NotLoaded;

        private void Awake()
        {
            currState = State.Loaded;
            DontDestroyOnLoad(gameObject);
            
            interstitialEmulator.gameObject.SetActive(false);
            rewardedEmulator.gameObject.SetActive(false);
        }

        public void CallAds(string unityCallerName, string unitId, bool isLoad, bool isInterstitial)
        {
            var unityCaller = GameObject.Find(unityCallerName);
            if(unityCaller == null) Debug.LogError($"Unity Caller {unityCallerName} not found", this);

            var type = (isInterstitial ? AdsType.Interstitial : AdsType.Rewarded).ToString();
            
            // Load
            if (isLoad)
            {
                LoadTask(unityCaller, unitId, type).Forget();
            }
            else
            {
                if (currState is State.NotLoaded or State.Loading)
                    SendMsg(unityCaller, AdsCallback.OnShow,
                        new AdsResponse(type, unitId, AdsCode.Failed, "Not Loaded"));
                else if (currState is State.Shown)
                    SendMsg(unityCaller, AdsCallback.OnShow,
                        new AdsResponse(type, unitId, AdsCode.Failed, "Already Shown"));
                else if (!showSuccess)
                    SendMsg(unityCaller, AdsCallback.OnShow,
                        new AdsResponse(type, unitId, AdsCode.Failed, "Failed by emulator logic"));
                else
                {
                    if (isInterstitial)
                        EmulatorTask(unityCaller, unitId, interstitialEmulator).Forget();
                    else
                        EmulatorTask(unityCaller, unitId, rewardedEmulator).Forget();
                }
            }
        }

        private async UniTaskVoid LoadTask(GameObject unityCaller, string unitId, string type)
        {
            var resp = new AdsResponse
            {
                type = type,
                unitId = unitId,
                code = loadSuccess ? AdsCode.Loaded : AdsCode.Failed,
                message = loadSuccess ? "Success by emulator logic" : "Failed by emulator logic"
            };
            
            // 이미 로딩 성공이라면,
            if (loadSuccess && currState == State.Loaded)
            {
                // 즉시 완료
                resp.code = AdsCode.Loaded;
                resp.message = "Already loaded";
                if(EventToInvoke.HasFlag(EventFlag.Loaded)) SendMsg(unityCaller, AdsCallback.OnLoad, resp);
                return;
            }
            
            // 로딩 시작
            currState = State.Loading;
            await UniTask.WaitForSeconds(delayTime);
            
            // 로딩 완료
            currState = State.Loaded;
            if(EventToInvoke.HasFlag(EventFlag.Loaded)) SendMsg(unityCaller, AdsCallback.OnLoad, resp);
        }

        private async UniTaskVoid EmulatorTask(GameObject unityCaller, string unitId, AdsEmulatorBase emulator)
        {
            var resp = new AdsResponse
            {
                type = AdsType.Rewarded.ToString(),
                unitId = unitId
            };
            
            // 리워드 캔버스 열기
            emulator.Open(unityCaller, unitId);
            currState = State.Shown;
            
            // 보여주기
            resp.code = AdsCode.Show;
            if(EventToInvoke.HasFlag(EventFlag.Show)) SendMsg(unityCaller, AdsCallback.OnEvent, resp);
            
            // 1프레임 대기
            await UniTask.DelayFrame(1);
            
            // 임프레션
            resp.code = AdsCode.Impression;
            if(EventToInvoke.HasFlag(EventFlag.Impression)) SendMsg(unityCaller, AdsCallback.OnEvent, resp);
        }

        public static void SendMsg(GameObject unityCaller, string methodName, AdsResponse resp) => unityCaller.SendMessage(methodName, JsonConvert.SerializeObject(resp));
    }
}
#endif