using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Accelib.Editor.Architecture
{
    /// <summary>빌드 크기 측정, 포맷, 히스토리 I/O 유틸리티</summary>
    public static class BuildSizeUtility
    {
        private const string HistoryDirRelative = "Library/Accelib/AutoBuild";
        private const string HistoryFileName = "build_size_history.json";

        /// <summary>지정 디렉토리의 총 파일 크기를 바이트 단위로 계산한다.</summary>
        public static long MeasureDirectorySize(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                return 0;

            return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Sum(f => new FileInfo(f).Length);
        }

        /// <summary>빌드 출력 경로(exe)에서 상위 디렉토리 전체 크기를 측정한다.</summary>
        public static long MeasurePlayerBuildSize(string buildOutputPath)
        {
            if (string.IsNullOrEmpty(buildOutputPath))
                return 0;

            var buildDir = Path.GetDirectoryName(buildOutputPath);
            return MeasureDirectorySize(buildDir);
        }

        /// <summary>바이트 수를 사람이 읽기 쉬운 문자열로 변환한다.</summary>
        public static string FormatBytes(long bytes)
        {
            if (bytes < 0) return "N/A";

            string[] units = { "B", "KB", "MB", "GB", "TB" };
            var size = (double)bytes;
            var unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return unitIndex == 0
                ? $"{bytes} B"
                : $"{size:F2} {units[unitIndex]}";
        }

        /// <summary>현재 크기와 이전 크기의 차이를 부호 포함 문자열로 변환한다.</summary>
        public static string FormatDiff(long currentBytes, long previousBytes)
        {
            if (previousBytes <= 0) return "(첫 빌드)";

            var diff = currentBytes - previousBytes;
            var absDiff = Math.Abs(diff);
            var percent = (double)diff / previousBytes * 100;
            var sign = diff >= 0 ? "+" : "-";

            return $"{sign}{FormatBytes(absDiff)} ({sign}{Math.Abs(percent):F1}%)";
        }

        /// <summary>히스토리 파일에서 이전 빌드 기록을 로드한다.</summary>
        public static BuildSizeRecord LoadPreviousRecord(string appId, string buildTarget)
        {
            var path = GetHistoryPath();
            if (!File.Exists(path)) return null;

            try
            {
                var json = File.ReadAllText(path);
                var history = JsonUtility.FromJson<BuildSizeHistory>(json);
                return history?.records?.Find(r =>
                    r.appId == appId && r.buildTarget == buildTarget);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"빌드 크기 히스토리 로드 실패: {e.Message}");
                return null;
            }
        }

        /// <summary>현재 빌드 기록을 히스토리 파일에 저장한다. 같은 appId+buildTarget 조합은 교체된다.</summary>
        public static void SaveRecord(BuildSizeRecord record)
        {
            var path = GetHistoryPath();
            BuildSizeHistory history;

            // 기존 히스토리 로드
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    history = JsonUtility.FromJson<BuildSizeHistory>(json) ?? new BuildSizeHistory();
                }
                catch
                {
                    history = new BuildSizeHistory();
                }
            }
            else
            {
                history = new BuildSizeHistory();
            }

            // 같은 appId+buildTarget 기록 교체
            history.records.RemoveAll(r =>
                r.appId == record.appId && r.buildTarget == record.buildTarget);
            history.records.Add(record);

            // 디렉토리 생성 후 저장
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var output = JsonUtility.ToJson(history, true);
            File.WriteAllText(path, output);
        }

        private static string GetHistoryPath()
        {
            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            return Path.Combine(projectRoot, HistoryDirRelative, HistoryFileName);
        }
    }
}
