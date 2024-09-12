using UnityEngine;

namespace Accelib.Extensions
{
    public static class CanvasGroupExtension
    {
        public static void SetInteractable(this CanvasGroup canvasGroup, bool interactable)
        {
            canvasGroup.blocksRaycasts = interactable;
            canvasGroup.interactable = interactable;
        } 
    }
}