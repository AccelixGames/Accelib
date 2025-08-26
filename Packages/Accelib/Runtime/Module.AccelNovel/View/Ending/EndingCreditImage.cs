using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Module.AccelNovel.View.Ending
{
    public class EndingCreditImage : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetSprite(UnityEngine.Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}