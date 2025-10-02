using System;
using System.Collections.Generic;
using Accelib.Module.AccelTag.TaggerSystem;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelTag
{
    [System.Serializable]
    public class TagFilter
    {
        // 모두 있어야 함, 하나라도 있어도 됨, 사용 안함
        [Header("혀용")]
        [SerializeField] private ETagRegex allowedRule;
        [AllowNesting]
        [HideIf(nameof(allowedRule), ETagRegex.UnUsed), SerializeField] private List<SO_AccelTag> allowed;
        
        [Header("비혀용")]
        [SerializeField] private ETagRegex disallowedRule;
        [AllowNesting]
        [HideIf(nameof(disallowedRule), ETagRegex.UnUsed), SerializeField] private List<SO_AccelTag> disallowed;

        [Header("상관관계")]
        [SerializeField] private ETagLogic logic;

        public bool Filter(ITagger tagger)
        {
            var allowedResult = allowedRule switch
            {
                ETagRegex.All => tagger?.AllTagsMatch(allowed),
                ETagRegex.Any => tagger?.AnyTagsMatch(allowed),
                _ => true
            } ?? allowed.Count <= 0;
            
            var disallowedResult = disallowedRule switch
            {
                ETagRegex.All => tagger?.AllTagsNoMatch(disallowed),
                ETagRegex.Any => tagger?.AnyTagsNoMatch(disallowed),
                _ => true
            } ?? true;

            return logic switch
            {
                ETagLogic.And => allowedResult && disallowedResult,
                ETagLogic.Or => allowedResult || disallowedResult,
                _ => false
            };
        }
        
        public enum ETagRegex
        {
            UnUsed = 0,
            Any = 1,
            All = 2
        }
        
        public enum ETagLogic
        {
            And = 0,
            Or = 1
        }
    }
}