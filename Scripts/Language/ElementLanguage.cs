using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    
    public class ElementLanguage : MonoBehaviour
    {
        #region Private Serializable Fields
        
        public Language currentLanguage;
        public Action<Language> OnChangeLanguage;

        [SerializeField] private Button languageButton;

        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI onStateText;
        [SerializeField] private TextMeshProUGUI offStateText;
        #endregion

        #region Private Fields
        
        private ButtonSwitch _languageButtonSwitch;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            onStateText.text = currentLanguage.ToString();
            offStateText.text = currentLanguage.ToString();
        }

        private void OnEnable()
        {
            languageButton.onClick.AddListener(OnClickLanguage);
            _languageButtonSwitch = languageButton.GetComponent<ButtonSwitch>();
        }

        private void OnDisable()
        {
            languageButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void OnClickLanguage()
        {
            OnChangeLanguage?.Invoke(currentLanguage);
            UGUIManager.Ins.GUILanguage.TurnOffElements();
            _languageButtonSwitch.SetState(ButtonSwitch.ButtonSwitchState.ON);
            
            // LeanLocalization.SetCurrentLanguageAll(currentLanguage.ToString());
        }

        #endregion

        #region Public Methods

        #endregion
    }
    
    public enum Language
    {
        English = 0,
        French = 1,
        German = 2,
        Italian = 3,
        Japanese = 4,
        Korean = 5,
        Portuguese = 6,
        Spanish = 7
    }
}