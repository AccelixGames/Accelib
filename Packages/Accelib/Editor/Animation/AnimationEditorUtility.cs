using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Accelix.Animation
{
    public static class AnimationEditorUtility
    {
        [MenuItem("Accelix/AddClipsToAnimator")]
        private static void AddClipsToAnimator()
        {
            var objects = Selection.objects;
            AnimatorController animator = null;
            var clips = new List<AnimationClip>();
            
            foreach (var obj in objects)
            {
                if (obj is AnimatorController ctrl) animator = ctrl;
                else if (obj is AnimationClip clip) clips.Add(clip);
            }
            
            if(animator == null) return;

            var animatorPath = AssetDatabase.GetAssetPath(animator);
            Debug.Log($"Animator: {animator.name}");

            foreach (var clip in clips)
            {
                Debug.Log($"Clip: {clip.name}");

                var newClip = Object.Instantiate(clip);
                newClip.name = clip.name;
                
                AssetDatabase.AddObjectToAsset(newClip, animatorPath);
            }
            //
            EditorUtility.SetDirty(animator);
            AssetDatabase.ImportAsset(animatorPath);
        }
        
        // 메뉴 항목의 유효성을 검사하는 메서드
        [MenuItem("Assets/Animator/ClearChild", true)]
        private static bool ValidateClearAnimControllerChild()
        {
            // 선택된 에셋이 애니메이터 컨트롤러인지 확인합니다.
            return Selection.activeObject is AnimationClip;
        }

        // 메뉴 항목을 클릭했을 때 실행되는 메서드
        [MenuItem("Assets/Animator/ClearChild", false, 1000)]
        private static void ClearAnimControllerChild()
        {
            // 선택된 애니메이터 컨트롤러를 가져옵니다.
            var clip = Selection.activeObject as AnimationClip;
            if(clip == null) return;
        
            AssetDatabase.RemoveObjectFromAsset(clip);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clip));
        }
    }
}