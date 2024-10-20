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
        private const string FOption = "저장 옵션";
        [Foldout(FOption)][SerializeField] private bool enableEncryption = true;
        [Foldout(FOption)][SerializeField] private string fileName;
        [Foldout(FOption)][SerializeField][ReadOnly] private string fileNameHash;
        [Foldout(FOption)][SerializeField][ReadOnly] private string remoteStorageName;

        private static IRemoteStorage _remoteStorage;
        private static IRemoteStorage _localStorage;
        private static SaveLoadConfig _config;
        
        protected abstract SaveDataBase SaveData { get; }
        protected string FilePath => $"SaveData/{fileNameHash}";

        // private void Awake() => TryInitialize();

        public async UniTask<bool> ReadAsync()
        {
            try
            {
                if (SaveData == null)
                    return Error("빈 데이터를 쓰려고 했습니다.");

                if (!TryInitialize())
                    return Error("초기화에 실패했습니다.");
                
#if UNITY_EDITOR
                if (_config.ForceNoRead)
                    return true;
#endif

                // 원격에서 읽기
                var result = await _remoteStorage.ReadAsync(FilePath);
                // 실패할 경우,
                if (!result.success)
                {
                    // 경고
                    Deb.LogWarning($"데이터 읽기({remoteStorageName})에 실패하여 로컬에 씁니다. 이유: {result.message}");
                    // 로컬에 쓰기
                    result = await _localStorage.ReadAsync(FilePath);
                }

                // 이것도 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 쓰기({remoteStorageName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    return false;
                }

                if (result.data == null)
                {
                    SaveData.New();
                    
#if UNITY_EDITOR
                    if (_config.PrintLog)
                        Deb.Log($"신규 데이터 읽기에 성공했습니다({remoteStorageName}): {FilePath}", this);
#endif
                }
                else
                {
                    // 복호화
                    var json = enableEncryption ? Crypto.DecryptFromBytes(result.data, _config.Secret) : Encoding.UTF8.GetString(result.data);
                    // 데이터 복구
                    SaveData.FromJson(json);
                
#if UNITY_EDITOR
                    if (_config.PrintLog)
                        Deb.Log($"데이터 읽기에 성공했습니다({remoteStorageName}): {FilePath}", this);
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
        }

        public async UniTask<bool> WriteAsync()
        {
            try
            {
                // 저장 데이터가 없을 경우,
                if (SaveData == null) return Error("빈 데이터를 쓰려고 했습니다.");
                // 초기화에 실패할 경우,
                if (!TryInitialize()) return Error("초기화에 실패했습니다.");
                
#if UNITY_EDITOR
                if (_config.ForceNoWrite)
                    return true;
#endif
                
                // json화
                var json = SaveData.ToJson();
                // 암호화
                var bytes = enableEncryption ? Crypto.EncryptToBytes(json, _config.Secret) : Encoding.UTF8.GetBytes(json);
                
                // 원격 저장소에 쓰기
                var result = await _remoteStorage.WriteAsync(bytes, FilePath);
                // 실패할 경우,
                if (!result.success)
                {
                    // 경고
                    Deb.LogWarning($"데이터 쓰기({remoteStorageName})에 실패하여 로컬에 씁니다. 이유: {result.message}");
                    // 로컬에 쓰기
                    result = await _localStorage.WriteAsync(bytes, FilePath);
                }

                // 이것도 실패할 경우,
                if (!result.success)
                {
                    // 에러
                    Deb.LogError($"데이터 쓰기({remoteStorageName})에 실패하였습니다. 이유: {result.message}");
                    // 리턴
                    return false;
                }

#if UNITY_EDITOR
                if (_config.PrintLog)
                    Deb.Log($"데이터 쓰기 성공했습니다({remoteStorageName}): {FilePath}", this);
#endif
                return true;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return false;
            }
        }
        
        private bool Error(string msg)
        {
            Deb.LogError($"{msg} : File({fileName}) Path({FilePath}) RemoteStorage({remoteStorageName})");
            return false;
        }

        private bool TryInitialize()
        {
            _config ??= SaveLoadConfig.Load();
            if (_config == null)
            {
                Deb.LogError("DataHandlerConfig 파일이 없습니다.", this);
                return false;
            }
            
            _remoteStorage ??= RemoteStorageSelector.GetRemoteStorage(_config.ForceLocalStorage);
            _localStorage ??= RemoteStorageSelector.GetLocalStorage();
            
            remoteStorageName = _remoteStorage?.Name;
            
            return _remoteStorage != null && _localStorage != null && _config != null;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                fileNameHash = $"f{Mathf.Abs(fileName.GetHashCode())}.save";
                gameObject.name = $"(SaveData) {fileName}";
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _remoteStorage = null;
            _localStorage = null;
            _config = null;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Read() => ReadAsync().Forget();
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test_Write() => WriteAsync().Forget();
#endif
    }
}