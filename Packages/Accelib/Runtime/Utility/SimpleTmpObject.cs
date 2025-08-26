using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleTmpObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;

        public void SetActive(string text, bool preDisable = false)
        {
            if(preDisable)
                gameObject.SetActive(false);
            
            tmp.text = text;
            gameObject.SetActive(true);
        }
        
        public async UniTask SetActiveDelayed(float delay, string text, bool preDisable = false)
        {
            await UniTask.WaitForSeconds(delay);
            
            SetActive(text, preDisable);
        }
    }
}