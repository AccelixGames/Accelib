using System;
using System.Collections.Generic;
using System.IO;
using Accelib.Extensions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.SaveLoad;
using Accelib.Utility;
using Accelix.Accelib.AccelNovel.Model;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace Accelib.Module.AccelNovel.Control.Utility
{
    public static class SaveUtility
    {
        private const string Di342Do = "D!<R" + "od(3" + "4Jdo" + "fel!";
        public const int MaxAutoSave = 5;

        private const string NewGameFlagKey = nameof(SaveUtility) + "-IsNewGame";
        private const string LatestSaveIdKey = nameof(SaveUtility) + "-LatestSave";
        private const string AutoSaveIndexKey = nameof(SaveUtility) + "-AutoSaveIndex";

// #if UNITY_EDITOR
//         public static string DirectoryPath => Path.Combine(Application.persistentDataPath, "SavesDir", "Editor");
// #else
//         public static string DirectoryPath => Path.Combine(Application.persistentDataPath, "SavesDir", "Local");
// #endif
        public static string DirectoryPath(string fileName = "") => SaveLoadSingleton.RemoteStorage.GetFilePath(fileName);

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Accelix/OpenSaveFileDirectory")]
        private static void OpenDirectory() => UnityEditor.EditorUtility.OpenWithDefaultApp(Path.Combine(Application.persistentDataPath, "SavesDir"));
#endif
        private static string CheckAndGetFilePath(int index, string extension)
        {
            // 디렉토리가 없으면 생성
            if (!Directory.Exists(DirectoryPath()))
                Directory.CreateDirectory(DirectoryPath());

            // 경로 반환
            return DirectoryPath($"save-{index:00}." + extension);
        }

        public static bool IsNewGame()
        {
            if (PlayerPrefs.GetInt(NewGameFlagKey, 1) == 1) return true;

            var latest = LoadLatest();
            return latest == null;
        }

        public static void SetNewGameFlag(bool enable) => PlayerPrefs.SetInt(NewGameFlagKey, enable ? 1 : 0);
        
        public static void SetLatestSaveIndex(int index) => PlayerPrefs.SetInt(LatestSaveIdKey, index);

        /// <summary>SaveData를 저장한다.</summary>
        public static bool Save(SaveData saveData, int slotIndex, Object caller = null)
        {
            // 저장데이터 NULL 이면 넘기기
            if (saveData == null) return false;

            try
            {
                // 파일경로 가져오기
                var filePath = CheckAndGetFilePath(slotIndex, "sav");

                // 프리뷰 이미지 저장
                if(saveData.preview != null) 
                    saveData.previewBytes = saveData.preview.EncodeToJPG();
                
                // json화 및 암호화
                saveData.createdAt = DateTimeExtension.UtcTotalSec();
                var json = JsonConvert.SerializeObject(saveData);
                var encrypt = Crypto.EncryptToBytes(json, Di342Do);

                // 파일 쓰기
                File.WriteAllBytes(filePath, encrypt);

                // 마지막 세이브 Index 저장 
                SetLatestSaveIndex(slotIndex);
            }
            catch (Exception e)
            {
                Deb.LogException(e, caller);
                return false;
            }

            return true;
        }

        /// <summary>SaveData를 자동 저장한다.</summary>
        public static bool AutoSave(SaveData saveData, Object caller = null)
        {
            // 저장데이터 NULL 이면 넘기기
            if (saveData == null) return false;

            // 자동저장 인덱스
            var autoSaveIndex = PlayerPrefs.GetInt(AutoSaveIndexKey, 0);
            if (autoSaveIndex is < 0 or >= MaxAutoSave) autoSaveIndex = 0;

            // 마지막 저장과 같다면, 종료
            var latest = LoadLatest(caller);
            if (saveData.IsSame(latest)) return false;

            // 저장
            var result = Save(saveData, autoSaveIndex, caller);
            if (result)
            {
                // 인덱스 상승
                autoSaveIndex = (autoSaveIndex + 1) % MaxAutoSave;
                PlayerPrefs.SetInt(AutoSaveIndexKey, autoSaveIndex);
            }

            return result;
        }

        /// <summary>SaveData를 로드한다.</summary>
        public static SaveData Load(int slotIndex, Object caller = null)
        {
            try
            {
                // 파일경로 가져오기
                var filePath = CheckAndGetFilePath(slotIndex, "sav");

                // 파일이 없으면, 종료
                if (!File.Exists(filePath))
                    return null;

                // 암호화된 데이터
                var encrypt = File.ReadAllBytes(filePath);
                var json = Crypto.DecryptFromBytes(encrypt, Di342Do);
                var saveData = JsonConvert.DeserializeObject<SaveData>(json);
                
                // 텍스쳐 로드
                if (saveData.previewBytes is { Length: > 0 })
                {
                    saveData.preview = new Texture2D(2, 2, DefaultFormat.HDR, TextureCreationFlags.None);
                    saveData.preview.LoadImage(saveData.previewBytes);
                }
                
                return saveData;
            }
            catch (Exception e)
            {
                Deb.LogException(e, caller);
                return null;
            }
        }

        /// <summary>가장 마지막 SaveData 를 로드한다.</summary>
        public static SaveData LoadLatest(Object caller = null)
        {
            // 마지막 인덱스 가져오기
            var index = PlayerPrefs.GetInt(LatestSaveIdKey, -1);
            if (index < 0) return null;

            // 로드
            return Load(index, caller);
        }

        /// <summary>SaveData를 여러개 로드한다.</summary>
        public static List<SaveData> LoadAll(int startIndex, int lenght, Object caller = null)
        {
            try
            {
                // 인덱스 익셉션
                if (startIndex < 0 || lenght <= 0) throw new IndexOutOfRangeException();

                // 데이터 리스트로 로드
                var result = new List<SaveData>();
                for (var i = startIndex; i < startIndex + lenght; i++)
                {
                    var data = Load(i, caller);
                    result.Add(data);
                }

                return result;
            }
            catch (Exception e)
            {
                Deb.LogException(e, caller);
                return null;
            }
        }
    }
}