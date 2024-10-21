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
        [SerializeField][ReadOnly] private string remoteStorageName;
        [SerializeField] [ReadOnly] private bool isDirty = false;
        [SerializeField] [ReadOnly] private bool isBlocked = false;

        private static IRemoteStorage _remoteStorage;
        private static SaveLoadConfig _config;
        
        protected abstract SaveDataBase SaveData { get; }

        public async UniTask<bool> ReadAsync()
        {
            // 저장 데이터가 없을 경우,
            if (SaveData == null) return Error("세이브 데이터가 없습니다.");
            // 초기화에 실패할 경우,
            if (!TryInitialize()) return Error("초기화에 실패했습니다.");
            // 잠긴 경우,
            if (isBlocked) return Error("현재 사용중입니다.");
#if UNITY_EDITOR
            if (_config.ForceNoRead)
                return true;
#endif

            try
            {
                // 잠그기
                isBlocked = true;

                // 원격에서 읽기
                var result = await _remoteStorage.ReadAsync(fileNameHash);
                // 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 읽기({remoteStorageName}, {fileName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    return false;
                }

                if (result.data == null)
                {
                    SaveData.New();

#if UNITY_EDITOR
                    if (_config.PrintLog)
                        Deb.Log($"신규 데이터 읽기에 성공했습니다({remoteStorageName}, {fileName}): {_remoteStorage.GetFilePath(fileNameHash)}", this);
#endif
                }
                else
                {
                    // 복호화
                    var json = enableEncryption
                        ? Crypto.DecryptFromBytes(result.data, _config.Secret)
                        : Encoding.UTF8.GetString(result.data);
                    // 데이터 복구
                    SaveData.FromJson(json);

#if UNITY_EDITOR
                    if (_config.PrintLog)
                        Deb.Log($"데이터 읽기에 성공했습니다({remoteStorageName}, {fileName}): {_remoteStorage.GetFilePath(fileNameHash)}", this);
#endif
                }

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

        public async UniTask<bool> WriteAsync()
        {
            // 저장 데이터가 없을 경우,
            if (SaveData == null) return Error("세이브 데이터가 없습니다.");
            // 초기화에 실패할 경우,
            if (!TryInitialize()) return Error("초기화에 실패했습니다.");
            // 잠긴 경우,
            if (isBlocked) return Error("현재 사용중입니다.");
#if UNITY_EDITOR
            if (_config.ForceNoWrite)
                return true;
#endif
            
            try
            {
                // 잠그기
                isBlocked = true;
                
                // json화
                var json = SaveData.ToJson();
                // 암호화
                var bytes = enableEncryption ? Crypto.EncryptToBytes(json, _config.Secret) : Encoding.UTF8.GetBytes(json);
                
                // 원격 저장소에 쓰기
                var result = await _remoteStorage.WriteAsync(bytes, fileNameHash);
                
                // 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 쓰기({remoteStorageName}, {fileName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    return false;
                }

#if UNITY_EDITOR
                if (_config.PrintLog)
                    Deb.Log($"데이터 쓰기에 성공했습니다({remoteStorageName}, {fileName}): {_remoteStorage.GetFilePath(fileNameHash)}", this);
#endif
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
        
        private bool Error(string msg)
        {
            Deb.LogError($"{msg} : File({fileName}) Path({_remoteStorage.GetFilePath(fileNameHash)}) RemoteStorage({remoteStorageName})");
            return false;
        }

        private bool TryInitialize()
        {
            _config ??= SaveLoadConfig.Load();
            if (!_config)
            {
                Deb.LogError("DataHandlerConfig 파일이 없습니다.", this);
                return false;
            }
            
            _remoteStorage ??= RemoteStorageSelector.GetRemoteStorage(_config.ForceLocalStorage);
            if (_remoteStorage == null)
            {
                Deb.LogError("RemoteStorage 를 불러올 수 없습니다.", this);
                return false;
            }
            
            remoteStorageName = _remoteStorage?.Name ?? "NULL";
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

        protected void SetDirty() => isDirty = true;

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
            _remoteStorage = null;
            _config = null;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Read() => ReadAsync().Forget();
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Write() => WriteAsync().Forget();
#endif
    }
}