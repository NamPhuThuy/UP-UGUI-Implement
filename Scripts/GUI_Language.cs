using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public class GUI_Language : GUIBase
    {
        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private ButtonClicky closeButton;
        [SerializeField] private ButtonClicky confirmButton;
        [SerializeField] private ButtonSwitch confirmButtonSwitch;

        [SerializeField] private ScrollRect scrollViewLanguage;
        [SerializeField] private List<ElementLanguage> elementList;
        [SerializeField] private List<ButtonSwitch> elementSwitchList;

        [Header("Flags")]
        [SerializeField] private Language currentLanguage;
        [SerializeField] private bool isConfirmButtonActive = false;

        public const Language DEFAULT_LANGUAGE = Language.English;
        #endregion

        #region Private Fields

        public static event Action LanguageChanged;
        
        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            closeButton.onClick.AddListener((() => { Hide(); }));
            confirmButton.onClick.AddListener(OnClickConfirm);

            
            // Disable Confirm Button
            confirmButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.OFF);
            isConfirmButtonActive = false;
            
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            foreach (ElementLanguage element in elementList)
            {
                element.OnChangeLanguage += OnChangeLanguage;
            }
        }

        private void OnDestroy()
        {
            foreach (ElementLanguage element in elementList)
            {
                element.OnChangeLanguage -= OnChangeLanguage;
            }
        }

        #endregion

        #region Button Events

        private void OnClickConfirm()
        {
            // Debug.Log($"GUILanguage.OnClickConfirm(), current lang: {currentLanguage}");
            if (!isConfirmButtonActive) return;

            Lean.Localization.LeanLocalization.SetCurrentLanguageAll(currentLanguage.ToString());
            Hide();

        }

        #endregion

        #region Private Methods

        private void OnChangeLanguage(Language newLanguage)
        {
            currentLanguage = newLanguage;
            LanguageChanged?.Invoke();

            Hide();
        }

        private void ActivateConfirmButton()
        {
            confirmButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
            isConfirmButtonActive = true;
        }

        private void DisableConfirmButton()
        {
            
        }



        #endregion

        #region Public Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            scrollViewLanguage.LockVerticalScrollALittle();
        }

        public void TurnOffElements()
        {
            foreach (ButtonSwitch element in elementSwitchList)
            {
                element.SetState(ButtonSwitch.ButtonSwitchState.OFF);
            }
        }

        #endregion
    }

}