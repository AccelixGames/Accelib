using System;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model
{
    [System.Serializable]
    public class LogData : IEquatable<LogData>
    {
        public Sprite portrait;
        public string name;
        public string text;
        public string voiceKey;

        public bool Equals(LogData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(portrait, other.portrait) && name == other.name && text == other.text;
        }
    }
}