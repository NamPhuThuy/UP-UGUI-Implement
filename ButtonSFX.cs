/*
Github: https://github.com/NamPhuThuy
*/

#if USE_AUDIO
using NamPhuThuy.AudioManage;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NamPhuThuy.UGUIImplement
{
    public class ButtonSFX : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        #region Private Serializable Fields
        
        // [Header("Audio Clips")]
        /*[SerializeField] private AudioEnum clickSound = AudioEnum.SFX_CONFIRM;
        [SerializeField] private AudioEnum hoverSound = AudioEnum.SFX_HIT;*/

        #endregion

        #region Private Fields

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // AudioManager.Ins.Play(clickSound);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // throw new System.NotImplementedException();
        }
        
        #endregion

        public void OnPointerEnter(PointerEventData eventData)
        {
            // AudioManager.Ins.Play(hoverSound);
        }

        #region Editor Methods

        public void ResetValues()
        {
            /*clickSound = AudioEnum.SFX_CONFIRM;
            hoverSound = AudioEnum.SFX_HIT;*/
        }

        #endregion
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ButtonSFX), true)]
    public class ButtonSFXInspector : Editor
    {
        private ButtonSFX _script;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
#endif
}