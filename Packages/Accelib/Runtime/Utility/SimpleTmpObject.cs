using TMPro;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleTmpObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;

        public void SetActive(string text)
        {
            tmp.text = text;
            gameObject.SetActive(true);
        }
    }
}