using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NamPhuThuy.Common;
using NamPhuThuy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UI
{

    public class GUIGallery : GUIBase
    {
        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button confirmButton;

        [Header("Components")]
        [SerializeField] private ScrollViewGallery scrollViewGallery;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            scrollViewGallery.InitContent();
            backButton.onClick.AddListener((() => Hide()));
            confirmButton.onClick.AddListener(OnClickConfirm);
        }

        void OnEnable()
        {
            
            StartCoroutine(IEInit());
            scrollViewGallery.ScrollRect.LockVerticalScrollALittle();
        }

        void OnDestroy()
        {
            backButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Button Events

        private void OnClickConfirm()
        {
            
        }
        
        #endregion

        #region Private Methods
        
        private IEnumerator IEInit()
        {
            yield return YieldHelper.Get(0.1f);

            ScrollTheViewToTop();
        }

        private void ScrollTheViewToTop()
        {
            scrollViewGallery.ScrollRect.verticalNormalizedPosition = 1f;
        }

        #endregion

        #region Public Methods
        #endregion
    }
}