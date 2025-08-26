using System.Collections.Generic;
using System.Linq;
using Accelib.Helper;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Action;
using Accelib.Module.AccelNovel.Control.Action.Dialogue;
using Accelib.Module.AccelNovel.Control.Parameter;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.SO;
using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
using Accelib.Module.Audio.Data._Base;
using Accelib.Module.SaveLoad;
using Accelib.Utility;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.AccelNovel.Control
{
    public class NovelController : MonoBehaviour
    {
        [Header("코어")]
        [SerializeField] private SO_NovelConfig config;
        [SerializeField] private SimpleTmpObject scnLabel;
        [SerializeField] private ParameterController paramController;
        public ParameterController Param => paramController;

        [Header("상태 컨트롤러")]
        [SerializeField] private BoolVariable isUILocked;
        [SerializeField] private BoolVariable isPaused;
        [SerializeField] private BoolVariable isHideUI;
        [SerializeField] private BoolVariable isAutoPlaying;
        [SerializeField] private BoolVariable isSkipLocked;
        [SerializeField] private FloatVariable autoPlaySpd;
        [SerializeField] private Timer autoplayTimer;
        [SerializeField] private Timer readyTimer;
        [SerializeField, ReadOnly] private bool isNewGame;
        [SerializeField, ReadOnly] private bool isReadyToPlay;
        [SerializeField, ReadOnly] private bool isGameEnd;
        [SerializeField, ReadOnly] private double startTimestamp;
        [SerializeField, ReadOnly] private bool isScnChange;
        
        [Header("액션 컨트롤러")]
        [SerializeField, ReadOnly] private SerializedDictionary<string, ActionController> actionControllers;
        [SerializeField, ReadOnly] private DialogueModule dialogueModule;

        [Header("플로우")]
        [SerializeField, ReadOnly] private SO_Scenario currScenario;
        [SerializeField, ReadOnly] private List<ActionLine> actionLines;
        [SerializeField, ReadOnly] private int currIndex;
        [SerializeField, ReadOnly] private ActionLine currAction;
        [SerializeField, ReadOnly] private ActionController currController;

        [Header("세이브 데이터")]
        [SerializeField, ReadOnly] private SaveData saveData;
        [SerializeField, ReadOnly] private int replayCount;
        private HashSet<string> _cgKeyList;
        public int ReplayCount => replayCount;
        public SO_Scenario CurrScenario => currScenario;
        public bool IsNewGame => isNewGame;

        [Header("SFX")]
        [SerializeField] private AudioRefBase voiceRef;
        // [SerializeField] private AudioRefSO saveSfx;
        private Sequence _seq;

        public void SetScnChange() => isScnChange = true;
        
        public int GetPlayDuration()
        {
            var currTimestamp = Time.realtimeSinceStartupAsDouble;
            return (int)(currTimestamp - startTimestamp);
        }
        
        private void Awake()
        {
            // 컨트롤러 캐싱
            actionControllers = new SerializedDictionary<string, ActionController>();
            foreach (var controller in GetComponentsInChildren<ActionController>()) 
                actionControllers.Add(controller.Keyword, controller);
            
            // 다이어로그 창 캐싱
            dialogueModule = GetComponentInChildren<DialogueModule>();
        }

        private async void Start()
        {
            // 시퀀스 킬
            _seq?.Kill();
            _seq = null;
            
            // 변수 초기화
            isScnChange = false;
            isReadyToPlay = false;
            isGameEnd = false;
            isSkipLocked.Value = false;
            isUILocked.Value = false;
            autoplayTimer.Stop();
            readyTimer.Clear();
            startTimestamp = Time.realtimeSinceStartupAsDouble;
            
            // 신규 게임인지?
            isNewGame = SaveUtility.IsNewGame(); 
            if (isNewGame)
            {
                // // 저장 데이터 로드
                // var holder = SaveLoadSingleton.GetHolder<SaveDataHolder_Stats>();
                // // 리플레이 카운트
                // replayCount = holder.GetReplayCount();
                replayCount = 0;

                // 첫 시나리오로 로드
                currScenario = config.GetFirstScenario();
                // 첫 라인 인덱스 로드
                currIndex = -1;
                
                // Deb.Log("New Game");
            }
            else
            {
                // 저장 데이터 로드
                saveData = SaveUtility.LoadLatest(this);
                replayCount = saveData.replay;
                
                // 현재 시나리오 로드
                currScenario = config.GetScenario(saveData.scnKey) ?? config.GetFirstScenario();
                // 현재 라인 인덱스 로드
                currIndex = saveData.lineIndex -1;
                
                // Deb.Log($"Load: {currScenario.ScnName} - {currIndex}번째 줄");
            }
            
            // CG Key List 초기화
            _cgKeyList = saveData?.cgKeyList?.ToHashSet();
            _cgKeyList ??= new HashSet<string>();
            
            // 파라메터 초기화
            paramController.Initialize(saveData);
            
            // 시나리오 파싱
            actionLines = ActionParser.ParseActions(currScenario);
            
            // 리소스 파싱 및 오토세이브 포인트 지정
            var spriteKeys = new HashSet<string>();
            var audioKeys = new HashSet<string>();
            for (var i = 0; i < actionLines.Count; i++)
            {
                var action = actionLines[i];
                if (actionControllers.TryGetValue(action?.key ?? string.Empty, out var controller))
                {
                    // 리소스 키 파싱
                    controller.ParseResources(action, ref spriteKeys, ref audioKeys);
                    
                    // 오토세이브 포인트 설정
                    if (controller.AutoSaveBeforeAction && i - 1 > 0)
                    {
                        var autoAction = actionLines[i - 1];
                        if (autoAction.key != action?.key)
                            autoAction.isAutoSavePoint = true;
                    }
                }
            }

            // 리소스 로딩
            await ResourceProvider.Instance.Load(spriteKeys, audioKeys);
            
            // 시나리오 라벨 재생
            if(currScenario.ShowNotice)
                scnLabel?.SetActiveDelayed(1.5f, currScenario.ScnName).Forget();
            
            // 액션 초기화
            foreach (var (_, controller) in actionControllers) 
                controller.Initialize();
            
            // 액션 롤백
            if (saveData != null)
                foreach (var (_, controller) in actionControllers)
                    if(saveData.actionState?.TryGetValue(controller.Keyword, out var json) ?? false)
                        if(!string.IsNullOrEmpty(json))
                            controller.FromJson(json);
            
            // 첫번째 재생
            PlayNext();
        }

        private void OnDisable()
        {
            // 컨트롤러 릴리즈
            Release();
            
            // 전역변수 초기화
            isPaused.Value = false;
            isUILocked.Value = false;
            
            // 리소스 릴리즈
            ResourceProvider.Instance.ReleaseAll();
        }

        public void Release()
        {
            // 릴리즈
            foreach (var (_, value) in actionControllers) 
                value.Release();
            actionControllers.Clear();
        }

        private void Update()
        {
            if (isGameEnd) return;
            if (!isReadyToPlay)
            {
                if (readyTimer.OnTime())
                    isReadyToPlay = true;
                return;
            }
            
            // 스페이스/엔터로 클릭
            // if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                // OnClick();
            
            // 오토플레이 조건 검사
            if (!isAutoPlaying.Value) return;
            if (isPaused.Value) return;
            if (isHideUI.Value) return;
            if (currController == null) return;
            if (currController.IsPlaying) return;
            if (!currController.CanSkip) return;
            
            var isVoicePlaying = AudioSingleton.IsPlaying(AudioChannel.Sfx_Voice);
            // 오토플레이 시간이 되었고 && 보이스 재생이 되고 있지 않을 때,
            if (!isVoicePlaying && autoplayTimer.OnTime(false, NovelUtility.MultiplierValue(autoPlaySpd.Value)))
                // 다음 재생
                PlayNext();
        }

        public void OnClick(BaseEventData e) 
        {
            // 마우스 포인터 데이터가 아니라면, 종료
            if (e is not PointerEventData eventData) return;
            // 좌클릭이 아니라면, 종료
            if (eventData.button != PointerEventData.InputButton.Left) return;

            OnClick();
        }

        public void OnClick(bool isFastForward = false)
        {
            // 준비 안되어있다면, 종료
            if (!isReadyToPlay) return;
            // 게임 종료되었다면, 종료
            if (isGameEnd) return;
            // 일시정지중이라면, 종료
            if (isPaused.Value) return;
            // 현재 액션이 없으면, 종료
            if (currController == null) return; 
            // 스킵 불가라면, 종료
            if (!currController.CanSkip) return;
            
            // 시퀀스가 있다면,
            if (_seq != null && _seq.IsActive() && _seq.IsPlaying())
            {
                // 시퀀스 스킵
                _seq?.Complete(true);
                return;
            }

            // 액션 수행중이라면,
            if (currController.IsPlaying)
            {
                // 스킵이 잠겨있지 않다면,
                if(!isSkipLocked.Value)
                    // 스킵
                    currController.SkipAction(isFastForward);
            }
            else
            {
                // 아니라면, 다음 재생
                PlayNext();
            }

            // 빨리감기일때 SFX 스킵
            if (isFastForward) 
                actionControllers["sfx"]?.SkipAction(true);
        }

        private void PlayNext(string jump = null)
        {
            // 게임 종료되었다면, 종료
            if(isGameEnd) return;
            
            // 더이상 액션이 없으면, 에러
            if (currIndex >= actionLines.Count)
            {
                Deb.LogWarning("시나리오가 끝났습니다.", this);
                return;
            }
            
            // 오토플레이 타이머 클리어
            autoplayTimer.Clear();
            
            // 인덱스 상승
            currIndex += 1;
            
            // 점프가 있으면,
            if(!string.IsNullOrEmpty(jump))
            {
                // 점프 인덱스 찾기
                var jumpIndex = actionLines.FindIndex(x => x.label == jump);
                if (jumpIndex < 0 || jumpIndex >= actionLines.Count)
                    Deb.LogError($"점프 포인트 {jump} 를 찾을 수 없습니다.", this);
                else
                    currIndex = jumpIndex; 
            }
            
            // 현재 액션 가져오기
            currAction = actionLines[currIndex];
            
            // 액션이 빈 액션이라면, 스킵
            if (!actionControllers.TryGetValue(currAction?.key ?? string.Empty, out currController))
            {
                Deb.LogError($"빈 액션!({currIndex}번째): [{currAction?.key}]{currAction?.value}", this);
                PlayNext();
                return;
            }
            
            // 자동저장 플래그가 서있다면, 저장
            if (currAction?.isAutoSavePoint ?? false)
                AutoSave();

            // 다이어로그 창 자체 표시
            _seq = dialogueModule.Show(!string.IsNullOrEmpty(currAction?.characterKey));
            
            // 액션 재생
            if (_seq == null)
            {
                currController.PlayAction(currAction, PlayNext);
            }
            else
            {
                _seq.OnComplete(() => currController.PlayAction(currAction, PlayNext));
            }
            
            if (currAction?.key == "ending")
            {
                EndGame();
            }
        }

        public static SaveData GetCurrentSaveData_Static() => FindFirstObjectByType<NovelController>(FindObjectsInactive.Exclude)?.GetCurrentSaveData();

        public SaveData GetCurrentSaveData()
        {
            // 저장 데이터 생성
            saveData = new SaveData
            {
                scnKey = currScenario.ScnKey,
                lineIndex = currIndex,
                preview = CameraCaptureModule.Capture(),
                
                actionState = new SerializedDictionary<string, string>(),
                
                affection = paramController.Affection.Value,
                replay = replayCount,
                playerName = paramController.PlayerName.Value,
                
                cgKeyList = _cgKeyList.ToList()
            };
            
            // 액션 컨트롤러를 순회하며,
            foreach (var (key, value) in actionControllers)
            {
                if(string.IsNullOrEmpty(key)) continue;

                // Json 생성 및 저장
                var json = value.ToJson();
                if (json != null) saveData.actionState[key] = json;
            }

            // 저장 데이터 반환
            return saveData;
        }

        public void AutoSave(string msg = "자동")
        {
            // 게임 종료되었다면, 종료
            if(isGameEnd) return;
            
            var result = SaveUtility.AutoSave(GetCurrentSaveData());
            if (result)
            {
                // Debug.Log("오토세이브!");
                // ToastSystem.Open($"{msg} 저장에 성공했습니다.");
                // saveSfx?.PlayOneShot();
            }
        }

        [Button]
        private void EndGame()
        {
            // 게임 종료되었다면, 종료
            if(isGameEnd) return;

            // 게임 종료 플래그 온
            isGameEnd = true;

            // 일시정지 온
            isPaused.Value = true;
        }
        
        public void OnViewCG(string actionValue) => _cgKeyList.Add(actionValue);
    }
}