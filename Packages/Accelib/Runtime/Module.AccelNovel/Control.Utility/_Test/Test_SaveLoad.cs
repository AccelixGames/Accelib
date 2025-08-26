using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Utility._Test
{
    public class Test_SaveLoad : MonoBehaviour
    {
        public int index;
        public SaveData dataToSave;
        public SaveData dataWhenLoad;

        [Header("SaveLoad")]
        public int idStart;
        public int idEnd;
        public List<SaveData> saveDataList;

        private void Start()
        {
            Deb.Log(SaveUtility.DirectoryPath());
        }

        [Button]
        private void Save()
        {
            var result = SaveUtility.Save(dataToSave, index, this);
            Deb.Log(result, this);
        }

        [Button]
        private void Load()
        {
            dataWhenLoad = SaveUtility.Load(index, this);
            Deb.Log(dataWhenLoad != null, this);
        }
        
        [Button]
        private void LoadAll()
        {
            saveDataList = SaveUtility.LoadAll(idStart, idEnd, this);
            Deb.Log(saveDataList != null, this);
        }
    }
}