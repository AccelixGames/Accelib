using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Accelib.Module.AccelNovel.View.Ending
{
    public class Ending_VoiceCard : MonoBehaviour
    {
        [Header("")]
        [SerializeField] private GameObject content;
        [SerializeField] private float nextActionDelayTime = 2f;

        [Header("재생될 오디오")]
        [SerializeField] private AudioRefSO voiceRef;

        public async UniTask Open()
        {
            if (voiceRef == null) return;

            gameObject.SetActive(true);
            
            // 모든 오디오 정지
            AudioSingleton.StopAllChannel(false);
            
            // 오디오 재생
            voiceRef.PlayOneShot();

            // 오디오 시간만큼 대기
            await UniTask.WaitForSeconds(voiceRef.Clip.length);
            
            // 콘텐츠 비활성화
            content.SetActive(false);
            
            // 다음 액션 대기
            await UniTask.WaitForSeconds(nextActionDelayTime);
        }
    }
}