using Accelib.Logging;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Reference.Model
{
    public abstract class SO_RefSingle<T> : SO_Ref<T> where T : Component
    {
        [Header("# 코어")]
        [SerializeField, ReadOnly] private T reference;
        
        public override T Reference => reference;

        public override  bool Register(T target)
        {
            if (reference && reference != target)
            {
                Deb.LogError($"이미 레퍼런스가 등록된 객체입니다({reference.name})", reference);
                return false;
            }
            
            reference = target;
            return true;
        }
        
        public override bool UnRegister(T target)
        {
            if (reference != target)
            {
                Deb.LogError($"자신의 레퍼런스만 취소할 수 있습니다({reference?.name})", reference);
                return false;
            }

            reference = null;
            return true;
        }
        
        protected const string MenuNamePrefix = _M + "Single-";
        protected const string FileNamePrefix = _F + "Single-";
        
        protected const int Order = BaseOrder;

        protected override void SetInitialValues() => reference = null;
    }
}