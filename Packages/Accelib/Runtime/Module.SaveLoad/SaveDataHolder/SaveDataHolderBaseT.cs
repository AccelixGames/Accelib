using Accelib.Module.SaveLoad.SaveData;
using UnityEngine;

namespace Accelib.Module.SaveLoad.SaveDataHolder
{
    public class SaveDataHolderBaseT<T> : SaveDataHolderBase where T : SaveDataBase
    {
        [Header("[현재 데이터]")]
        [SerializeField] protected T data;

        protected override SaveDataBase SaveData => data;
    }
}