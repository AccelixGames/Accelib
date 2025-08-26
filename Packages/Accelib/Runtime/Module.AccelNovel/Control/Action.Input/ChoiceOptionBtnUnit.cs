using Accelib.Module.UI.Utility;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.AccelNovel.Control.Action.Input
{
    public class ChoiceOptionBtnUnit : MonoBehaviour
    {
        [Header("연결")]
        [SerializeField] private AnimationButton animBtn;
        [SerializeField] private TMP_Text indexTMP;
        [SerializeField] private TMP_Text textTMP;
        
        [Header("핸들러")]
        [SerializeField, ReadOnly] private ActionController_Choice choiceController;
        [SerializeField, ReadOnly] private int id;
        
        public void Open(int index, string text, ActionController_Choice ctrl)
        {
            id = index;
            indexTMP.text = (index+1).ToString();
            textTMP.text = text;

            choiceController = ctrl;
            
            animBtn.SetEnabled(true);
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1 + id)) 
                //OnClick();
                animBtn.OnPointerClick(new PointerEventData(EventSystem.current){button = PointerEventData.InputButton.Left});
        }

        public void Hide()
        {
            if(gameObject.activeSelf)
                animBtn.SetEnabled(false);
        }

        public void OnClick()
        {
            if (!animBtn.isActiveAndEnabled) return;
            if (!animBtn.IsEnabled) return;
            
            choiceController?.OnClickOption(id);
        }
    }
}