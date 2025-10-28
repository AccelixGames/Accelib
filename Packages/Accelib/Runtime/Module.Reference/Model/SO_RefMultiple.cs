using System.Collections.Generic;
using System.Linq;
using Accelib.Extensions;
using Accelib.Logging;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Reference.Model
{
    public abstract class SO_RefMultiple<T> : SO_Ref<T> where T : Component
    {
        [Header("# 코어")]
        [SerializeField] private ReferenceProvideMode mode = ReferenceProvideMode.Random;
        [SerializeField, ReadOnly] private List<T> referenceList = new();

        public IReadOnlyList<T> ReferenceList => referenceList;
        
        public override T Reference => mode switch
        {
            ReferenceProvideMode.Random => referenceList.GetRandom(),
            ReferenceProvideMode.First => referenceList.FirstOrDefault(),
            ReferenceProvideMode.Last => referenceList.LastOrDefault(),
            _ => null
        };

        public override bool Register(T target)
        {
            if (referenceList.Contains(target))
            {
                Deb.LogError($"이미 레퍼런스가 등록된 객체입니다({target.name})", target);
                return false;
            }
            
            referenceList.Add(target);
            return true;
        }

        public override bool UnRegister(T target) => referenceList.Remove(target);

        private enum ReferenceProvideMode
        {
            Random = 0,
            First = 1, 
            Last = 2, 
        }
        
        protected const string MenuNamePrefix = _M + "Multiple-";
        protected const string FileNamePrefix = _F + "Multiple-";
        
        protected const int Order = BaseOrder + 50;

        protected override void SetInitialValues() => referenceList = new List<T>();
    }
}