using Accelib.Module.UI.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Utility.Animation
{
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(-int.MaxValue)]
    public class ResetAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        //[SerializeField] private LifeCycleType lifeCycle;

        private void Awake()
        {
            animator.writeDefaultValuesOnDisable = true;
        }
        
        // private void Awake()
        // {
        //     if(lifeCycle == LifeCycleType.Awake)
        //         ResetAnim();
        // }
        // private void Start()
        // {
        //     if(lifeCycle == LifeCycleType.Start)
        //         ResetAnim();
        // }
        // private void OnEnable()
        // {
        //     if(lifeCycle == LifeCycleType.OnEnable)
        //         ResetAnim();
        // }
        // private void OnDisable()
        // {
        //     if(lifeCycle == LifeCycleType.OnDisable)
        //         ResetAnim();
        // }
        // private void OnDestroy()
        // {
        //     if(lifeCycle == LifeCycleType.OnDestroy)
        //         ResetAnim();
        // }
        //
        // [Button]
        // public void ResetAnim()
        // {
        //     animator.Rebind();
        //     animator.Update(0f);
        // }

        private void Reset()
        {
            animator =  GetComponent<Animator>();
        }
    }
}