using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.SO
{
    [CreateAssetMenu(fileName = "TempDialogue", menuName = "Scriptable Objects/TempDialogue")]
    public class TempDialogueSO : ScriptableObject
    {
        public bool stateChange;
        public bool player;
        //public EMaidAnimStateTemp state;
        public string stateName;

        public string who;
        [TextArea] public string script;
    }
}
