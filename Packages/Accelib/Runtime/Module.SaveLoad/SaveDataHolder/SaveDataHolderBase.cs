using System;
using System.Text;
using Accelib.Logging;
using Accelib.Module.SaveLoad.Config;
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.SaveData;
using Accelib.Utility;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.SaveLoad.SaveDataHolder
{
    public abstract class SaveDataHolderBase : MonoBehaviour
    {
        [Header("공통 옵션")]
        [SerializeField] private bool enableEncryption = true;
        [SerializeField] private string fileName;
        [SerializeField][ReadOnly] private string fileNameHash;
        [SerializeField] [ReadOnly] private bool isDirty = false;
        [SerializeField] [ReadOnly] private bool isBlocked = false;

        
        protected abstract SaveDataBase SaveData { get; }

        private byte[] _lastWrittenBytes = null;

        public async UniTask<bool> ReadAsync()
        {
            // 저장 데이터가 없을 경우,
            if (SaveData == null) throw ErrException("세이브 데이터가 없습니다.");
            // 초기화에 실패할 경우,
            if (!IsInitialized()) throw ErrException("초기화에 실패했습니다.");
            // 잠긴 경우,
            if (isBlocked) throw ErrException("현재 사용중입니다.");
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // 읽기 강제 무시
            if (SaveLoadSingleton.Config.ForceNoRead) return true;
#endif

            try
            {
                // 잠그기
                isBlocked = true;

                // 원격에서 읽기
                var result = await SaveLoadSingleton.RemoteStorage.ReadAsync(fileNameHash);
                // 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 읽기({fileName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    SaveData.New();
                    return true;
                }

                if (result.data == null)
                {
                    SaveData.New();

#if UNITY_EDITOR
                    if (SaveLoadSingleton.Config.PrintLog)
                        Deb.Log($"신규 데이터 읽기에 성공했습니다({fileName}): {SaveLoadSingleton.RemoteStorage.GetFilePath(fileNameHash)}", this);
#endif
                }
                else
                {
                    // 복호화
                    var json = enableEncryption
                        ? Crypto.DecryptFromBytes(result.data, SaveLoadSingleton.Config.Secret)
                        : Encoding.UTF8.GetString(result.data);
                    // 데이터 복구
                    SaveData.FromJson(json);

#if UNITY_EDITOR
                    if (SaveLoadSingleton.Config.PrintLog)
                        Deb.Log($"데이터 읽기에 성공했습니다({fileName}): {SaveLoadSingleton.RemoteStorage.GetFilePath(fileNameHash)}", this);
#endif
                }

                // 성공
                return true;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                SaveData.New();
                return true;
            }
            finally
            {
                isBlocked = false;
            }
        }

        public async UniTask<bool> WriteAsync()
        {
            // 저장 데이터가 없을 경우,
            if (SaveData == null) throw ErrException("세이브 데이터가 없습니다.");
            // 초기화에 실패할 경우,
            if (!IsInitialized()) throw ErrException("초기화에 실패했습니다.");
            // 잠긴 경우,
            if (isBlocked) throw ErrException("현재 사용중입니다.");
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // 쓰기 강제 무시
            if (SaveLoadSingleton.Config.ForceNoWrite) return true;
#endif
            
            try
            {
                // 잠그기
                isBlocked = true;
                
                // json화
                var json = SaveData.ToJson();
                // 암호화
                var bytes = enableEncryption ? Crypto.EncryptToBytes(json, SaveLoadSingleton.Config.Secret) : Encoding.UTF8.GetBytes(json);

                // 이전에 쓴 데이터와 같다면, 종료
                if (_lastWrittenBytes == bytes) return true;
                
                // 원격 저장소에 쓰기
                var result = await SaveLoadSingleton.RemoteStorage.WriteAsync(bytes, fileNameHash);
                
                // 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 쓰기({fileName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    return true;
                }

#if UNITY_EDITOR
                if (SaveLoadSingleton.Config.PrintLog)
                    Deb.Log($"데이터 쓰기에 성공했습니다({fileName}): {SaveLoadSingleton.RemoteStorage.GetFilePath(fileNameHash)}", this);
#endif
                // 캐싱
                _lastWrittenBytes = bytes;
                
                // 성공
                return true;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return false;
            }
            finally
            {
                isBlocked = false;
            }
        }
        
        private Exception ErrException(string msg) => new($"{msg} : File({fileName}) Path({SaveLoadSingleton.RemoteStorage.GetFilePath(fileNameHash)})");

        private bool IsInitialized()
        {
            if (SaveLoadSingleton.Config == null)
            {
                Deb.LogError("DataHandlerConfig 파일이 없습니다.", this);
                return false;
            }
            
            if (SaveLoadSingleton.RemoteStorage == null)
            {
                Deb.LogError("RemoteStorage 를 불러올 수 없습니다.", this);
                return false;
            }

            return true;
        }

        private void LateUpdate()
        {
            // 더티 플레그가 켜져있다면,
            if (isDirty && !isBlocked)
            {
                // 더티 플래그 꺼주기
                isDirty = false;
                // 데이터 쓰기
                WriteAsync().Forget();
            }
        }

        public void SetDirty() => isDirty = true;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                fileNameHash = $"f{Mathf.Abs(fileName.GetHashCode())}.sav";
                gameObject.name = $"(SaveData) {fileName}";
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            SaveLoadSingleton.RemoteStorage = null;
            SaveLoadSingleton.Config = null;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Read() => ReadAsync().Forget();
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Write() => WriteAsync().Forget();
#endif
    }
}