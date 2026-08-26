using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class GUIReward : GUIBase
    {
        [Header("Components")]
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform[] rewardItemRTs;

        #region Serializable Fields

        [SerializeField] private List<int> currentRewardList;

        [SerializeField] private Button okButton;
        [SerializeField] private Button closeButton;

        #endregion

        #region Private Fields

        private GUIRewardItem[] _rewardItems;

        private void SetupRewards(List<int> rewardList)
        {
            List<RectTransform> validResourceGroups = new List<RectTransform>();

        }


        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            okButton.onClick.AddListener(OnClickConfirm);
            closeButton.onClick.AddListener(OnClickConfirm);
            
            _rewardItems = new GUIRewardItem[rewardItemRTs.Length];

            for (int i = 0; i < rewardItemRTs.Length; i++)
            {
                _rewardItems[i] = rewardItemRTs[i].GetComponent<GUIRewardItem>();
            }
        }

        private void OnDestroy()
        {
            okButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        #endregion



        #region Button Events

        private void OnClickConfirm()
        {
            Hide();

           
        }

        #endregion

        #region Override Methods

        public override void Show(params object[] parameters)
        {
            base.Show(parameters);

            if (parameters != null)
            {
                currentRewardList = (List<int>)parameters[0];
            }
            SetupRewards(currentRewardList);
            // SetupRewards2(currentRewardList);
        }



        public override void Hide(params object[] parameters)
        {
            base.Hide(parameters);
            // DataManager.Ins.PlayerData.TryApplyRewards(currentRewardList);

        }

        #endregion


    }
}
