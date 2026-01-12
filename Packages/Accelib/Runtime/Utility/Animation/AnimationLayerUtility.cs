using UnityEngine;

namespace Accelib.Utility.Animation
{
    public class AnimationLayerUtility : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private int layerIndex;
		[SerializeField, Range(0f,1f)] private float layerWeight;

        public void SetLayerWeight()
        {
            animator.SetLayerWeight(layerIndex, layerWeight);
        }

		public void SetLayerWeight(float weight)
        {
			layerWeight = weight;
            SetLayerWeight();
        }
    }
}