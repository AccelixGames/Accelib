#if UNITY_SWITCH
using System;
using System.Collections.Generic;
using System.Text;
using Accelib.Logging;
using Newtonsoft.Json;
using UnityEngine.Switch;

namespace Accelib.Module.Prefs.Wrapper
{
    public class PlayerPrefsWrapper_Switch : PlayerPrefsWrapper
    {
        private RawDataGroup _rawDataGroup;
        
        public PlayerPrefsWrapper_Switch() => ReadRawData();

        private void ReadRawData()
        {
            try
            {
                var rawData = PlayerPrefsHelper.rawData;
                if (rawData == null || rawData.Length == 0)
                {
                    _rawDataGroup = new RawDataGroup();
                    return;
                }
                
                var json = Encoding.UTF8.GetString(rawData);
                _rawDataGroup = JsonConvert.DeserializeObject<RawDataGroup>(json);
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                _rawDataGroup = new RawDataGroup();
            }
        }

        private void WriteRawData()
        {
            try
            {
                if (_rawDataGroup == null)
                {
                    PlayerPrefsHelper.rawData = null;
                    return;
                }
                
                var json = JsonConvert.SerializeObject(_rawDataGroup);
                var bytes = Encoding.UTF8.GetBytes(json);
                PlayerPrefsHelper.rawData = bytes;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
            }
        }
        
        public override int GetInt(string key, int defaultValue = 0) => _rawDataGroup.IntDict.GetValueOrDefault(key, defaultValue);
        public override void SetInt(string key, int value) => _rawDataGroup.IntDict[key] = value;
        
        public override float GetFloat(string key, float defaultValue = 0) => _rawDataGroup.FloatDict.GetValueOrDefault(key, defaultValue);
        public override void SetFloat(string key, float value) => _rawDataGroup.FloatDict[key] = value;
        
        public override string GetString(string key, string defaultValue = "") => _rawDataGroup.StringDict.GetValueOrDefault(key, defaultValue);
        public override void SetString(string key, string value) => _rawDataGroup.StringDict[key] = value;
        
        public override void Save() => WriteRawData();

        [Serializable]
        private class RawDataGroup
        {
            public Dictionary<string, int> IntDict = new();
            public Dictionary<string, float> FloatDict = new();
            public Dictionary<string, string> StringDict = new();
        }
    }
}
#endif