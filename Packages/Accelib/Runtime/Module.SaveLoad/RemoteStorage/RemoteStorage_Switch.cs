#if UNITY_SWITCH
using System;
using Accelib.Logging;
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;
using nn.fs;
using UnityEngine.Switch;

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Switch : IRemoteStorage
    {
        private readonly bool _isInitialized;
        
        public string Name => "Switch";
        private const string MountName = "SavesDir";

        public RemoteStorage_Switch()
        {
            try
            {
                _isInitialized = false;
                Deb.Log("RemoteStorage_Switch init started");
            
                // 계정 라이브러리는 애플리케이션 전체에서 한 번만 초기화하면 됩니다.
                nn.account.Account.Initialize();
                Deb.Log("nn.account.Account.Initialize()");
            
                // 유저 계정을 invalid 상태로 가져옵니다. (초기화)
                var userId = nn.account.Uid.Invalid;
                // 유저 핸들을 생성합니다.
                var userHandle = new nn.account.UserHandle();

                // 시작 사용자 계정이 필수로 설정되었다고 가정합니다.
                // 애플리케이션 시작 전에 선택된 사용자를 엽니다.
                if (!nn.account.Account.TryOpenPreselectedUser(ref userHandle))
                    // 리테일 버전에서는 발생하지 않아야 합니다.
                    throw new Exception("nn.account.Account.TryOpenPreselectedUser failed");

                // 미리 선택된 사용자 계정의 사용자 ID를 가져옵니다.
                var result = nn.account.Account.GetUserId(ref userId, userHandle);

                // 오류 발생 시 종료합니다.
                if (!result.IsSuccess())
                    throw new Exception(result.ToString());
                
                // 세이브 데이터를 마운트합니다.
                result = nn.fs.SaveData.Mount(MountName, userId);
                
                // 오류 발생 시 종료합니다.
                if (!result.IsSuccess())
                    throw new Exception(result.ToString());
                
                Deb.Log($"User: {userId}, Result: {result.ToString()}");
                _isInitialized = true;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                _isInitialized = false;
            }
        }

        public string GetFilePath(string fileName) => $"{MountName}:/{fileName}";

        public async UniTask<RemoteTaskResult> WriteAsync(byte[] data, string fileName)
        {
            if(!_isInitialized)
                return RemoteTaskResult.Failed("RemoteStorage_Switch is not initialized");
            
            try
            {
                // 이 메서드는 저장 중에 사용자가 게임을 종료하지 못하도록 방지합니다.
                // 이 메서드는 메인 스레드에서 호출되어야 합니다.
                Notification.EnterExitRequestHandlingSection();

                // nn.Result 객체는 NintendoSDK 플러그인 작업의 결과를 얻는 데 사용됩니다.
                nn.Result result;

                // 파일명을 마운트 이름에 추가하여 저장 데이터 파일의 전체 경로를 가져옵니다.
                var filePath = GetFilePath(fileName);

                // NintendoSDK 플러그인은 파일 작업을 위해 FileHandle 객체를 사용합니다.
                var handle = new FileHandle();

                while (true)
                {
                    // 파일을 쓰기 모드로 열려고 시도합니다.
                    result = File.Open(ref handle, filePath, OpenFileMode.Write);
                    // 파일이 성공적으로 열렸는지 확인합니다.
                    if (result.IsSuccess())
                        // 파일이 성공적으로 열렸으므로 루프를 종료합니다.
                        break;

                    if (FileSystem.ResultPathNotFound.Includes(result))
                    {
                        // 해당 파일 경로에 항목이 없는 경우, 인코딩된 데이터 크기로 파일을 생성합니다.
                        result = File.Create(filePath, data.LongLength);

                        // 파일이 성공적으로 생성되었는지 확인합니다.
                        if (!result.IsSuccess())
                            return RemoteTaskResult.Failed($"Unable to open {filePath}: {result.ToString()}");
                    }
                    else
                    {
                        // 디버깅 목적의 일반적인 오류 처리.
                        return RemoteTaskResult.Failed($"Failed to open {filePath}: {result.ToString()}");
                    }
                }

                // 파일을 이진 데이터 크기로 설정합니다.
                result = File.SetSize(handle, data.LongLength);

                // 충분한 공간이 없을 경우 오류 처리가 필요하지 않다면 이 오류를 처리할 필요는 없습니다.
                if (FileSystem.ResultUsableSpaceNotEnough.Includes(result))
                {
                    File.Close(handle);
                    return RemoteTaskResult.Failed($"Insufficient space to write {data.LongLength} to {filePath}");
                }

                // 참고: File.Write()를 WriteOption.Flush와 함께 호출하면 두 번의 쓰기 작업이 발생합니다.
                result = File.Write(handle, 0, data, data.LongLength, WriteOption.Flush);

                // 충분한 공간이 없을 경우, nn.fs.OpenFileMode.AllowAppend을 사용하지 않는다면 이 오류를 처리할 필요는 없습니다.
                if (FileSystem.ResultUsableSpaceNotEnough.Includes(result))
                    Deb.LogError($"Insufficient space to write {data.LongLength} bytes to {filePath}");

                // 파일은 커밋하기 전에 반드시 닫아야 합니다.
                File.Close(handle);

                // 커밋하기 전에 쓰기 작업이 성공했는지 확인합니다.
                if (!result.IsSuccess())
                    return RemoteTaskResult.Failed($"Failed to write {filePath}: {result.ToString()}");

                // 이 메서드는 저널링 영역에서 메인 저장소 영역으로 데이터를 이동합니다.
                // 이 메서드를 호출하지 않으면 애플리케이션이 종료될 때 모든 변경 사항이 사라집니다.
                // 모든 이전 작업이 성공했을 때만 이 메서드를 호출하세요.
                result = FileSystem.Commit(MountName);

                return new RemoteTaskResult(result.IsSuccess(), data, result.ToString());
            }
            catch (Exception e)
            {
                // 예외가 발생하면 로그를 기록합니다.
                Deb.LogException(e);
                return RemoteTaskResult.Failed(e.Message);
            }
            finally
            {
                // 저장 중에도 시스템이 게임을 종료하는 것을 방지하지 않도록 설정합니다.
                Notification.LeaveExitRequestHandlingSection();
            }
        }

        public async UniTask<RemoteTaskResult> ReadAsync(string fileName)
        {
            if(!_isInitialized)
                return RemoteTaskResult.Failed("RemoteStorage_Switch is not initialized");
            
            try
            {
                // 파일명을 마운트 이름에 추가하여 저장 데이터 파일의 전체 경로를 가져옵니다.
                var filePath = GetFilePath(fileName);

                // NintendoSDK 플러그인은 파일 작업을 위해 FileHandle 객체를 사용합니다.
                var handle = new FileHandle();
                
                // 파일을 읽기 전용 모드로 열려고 시도합니다.
                var result = File.Open(ref handle, filePath, OpenFileMode.Read);
                // 찾기에 실패할 경우,
                if (!result.IsSuccess())
                    return RemoteTaskResult.Failed(FileSystem.ResultPathNotFound.Includes(result)
                        ? $"File not found: {filePath}"
                        : $"Unable to open {filePath}: {result.ToString()}");

                // 파일 크기를 가져옵니다.
                long fileSize = 0;
                File.GetSize(ref fileSize, handle);

                // 저장 데이터를 버퍼에 읽어옵니다.
                var bytes = new byte[fileSize];
                File.Read(handle, 0, bytes, fileSize);

                // 파일을 닫습니다.
                File.Close(handle);

                // 결과를 반환합니다.
                return new RemoteTaskResult(true, bytes);
            }
            catch (Exception e)
            {
                // 예외가 발생하면 로그를 기록합니다.
                Deb.LogException(e);
                return RemoteTaskResult.Failed(e.Message);
            }
        }

        /// <summary>
        /// 저장 데이터 아카이브를 마운트 해제하고 모든 리소스를 해제합니다.
        /// 애플리케이션이 종료될 때 시스템에 의해 저장 데이터 드라이브는 자동으로 마운트 해제됩니다.
        /// 애플리케이션 전체 기간 동안 저장 데이터를 사용하고 싶다면, 이 메서드를 호출할 필요가 없습니다.
        /// </summary>
        public static void Cleanup()
        {
            Deb.Log("Unmounting save data archive");
            FileSystem.Unmount(MountName);
        }

        private const string PrefsFileName = "playerprefs";
        
        public async UniTaskVoid WritePlayerPrefs()
        {
            var data = PlayerPrefsHelper.rawData;
            var result = await WriteAsync(data, PrefsFileName);
            if (result.success) 
                Deb.Log($"PlayerPrefs({PrefsFileName}) Write success: {result.message}");
            else
                Deb.LogError($"PlayerPrefs({PrefsFileName}) Write failed: {result.message}");
        }

        public async UniTaskVoid ReadPlayerPrefs()
        {
            var result = await ReadAsync(PrefsFileName);
            if (result.success)
            {
                PlayerPrefsHelper.rawData = result.data;
                Deb.Log($"PlayerPrefs({PrefsFileName}) Read success: {result.message}");
            }
            else
                Deb.LogError($"PlayerPrefs({PrefsFileName}) Read failed: " + result.message);
        }
    }
}
#endif