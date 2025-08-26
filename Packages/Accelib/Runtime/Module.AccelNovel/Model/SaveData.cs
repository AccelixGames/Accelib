using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model
{
    [Serializable]
    public class SaveData
    {
        [Header("메타데이터")]
        public bool isAutoSave = false;
        public long createdAt = 0;
        
        [NonSerialized]
        public Texture2D preview;
        public byte[] previewBytes;

        [Header("시나리오")]
        public string scnKey;
        public int lineIndex;
        public SerializedDictionary<string, string> actionState;

        [Header("변수")]
        public int replay = 0;
        public int affection = 0;
        public string playerName;

        [Header("엔딩용 CG")]
        public List<string> cgKeyList;
        
        public bool IsSame(SaveData other)
        {
            // NULL 체크
            if (other == null) return false;
            
            // 메타데이터
            if (isAutoSave != other.isAutoSave) return false;
            
            // 시나리오
            if (scnKey != other.scnKey) return false;
            if (lineIndex != other.lineIndex) return false;
            if (!DictIsSame(actionState, other.actionState)) return false;
            
            // 변수
            if (affection != other.affection) return false;
            if (playerName != other.playerName) return false;

            // 같아요!
            return true;
        }

        private bool DictIsSame(SerializedDictionary<string, string> lhs, SerializedDictionary<string, string> rhs)
        {
            if (lhs == null && rhs == null) return true;
            if (lhs != null && rhs != null)
            {
                if(lhs.Count != rhs.Count) return false;
                foreach (var (key, l) in lhs)
                {
                    if (!rhs.TryGetValue(key, out var r)) return false;
                    if (l != r) return false;
                }

                return true;
            }
            
            return false;
        }
    }
}