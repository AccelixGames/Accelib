using UnityEngine;

namespace Accelib.Module.Prefs.Wrapper
{
    public class PlayerPrefsWrapper
    {
        public virtual int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public virtual void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        
        public virtual float GetFloat(string key, float defaultValue = 0) => PlayerPrefs.GetFloat(key, defaultValue);
        public virtual void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        
        public virtual string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);
        public virtual void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        
        public virtual void Save() => PlayerPrefs.Save();
    }
}