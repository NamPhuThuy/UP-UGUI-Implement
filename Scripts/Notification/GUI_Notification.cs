using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.AdNetworkAdapter;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class GUI_Notification : GUIBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button confirmButton;

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            confirmButton.onClick.AddListener(() => Hide());
        }
        
        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            titleText.text = (string)parameters[0];
            descriptionText.text = (string)parameters[1];
            
            AdsManager.Ins.Hide_MRec_MAX();
        }

        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);

            /*if (GamePersistentVariable.isPauseCountingRevive)
            {
                GamePersistentVariable.isPauseCountingRevive = false;
            }*/
        }

        #endregion

        private int GetActiveChildCount(Transform parent)
        {
            int count = 0;

            foreach (Transform child in parent)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
