using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using NamPhuThuy.DataManage;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class BoosterPanel : MonoBehaviour, MMEventListener<EBoosterDataUpdated>
    {
        #region Private Serializable Fields

        [Header("Flags")]
        [SerializeField] private BoosterType boosterType;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI boosterCountText;

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            UpdateUI();
        }

        private void OnEnable()
        {
            MMEventManager.RegistCurrentEvents(this);
        }

        private void OnDisable()
        {
            MMEventManager.UnregistCurrentEvents(this);
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            boosterCountText.text = DataManager.Ins.PInventoryData.GetBoosterNum(boosterType).ToString();
        }

        
        #endregion

        #region Public Methods


        #endregion

        #region Events Listen

        public void OnMMEvent(EBoosterDataUpdated eventArgs)
        {
            if (eventArgs.BoosterType == boosterType)
            {
                UpdateUI();
            }
        }

        #endregion
    }
}