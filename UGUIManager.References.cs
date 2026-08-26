using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.Common;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{

    public partial class UGUIManager 
    {
        /*
        Performance Comparison:

        1. Serialized Fields:
        - **Pros**: Fast direct access, editor-friendly, type-safe.
        - **Cons**: Poor scalability, hardcoded, manual iteration for all GUIs.
        - **Performance**: Best for small, static GUI setups.

        2. Dictionary-Based:
        - **Pros**: Dynamic mapping, scalable, flexible.
        - **Cons**: Runtime overhead for initialization, type casting, debugging complexity.
        - **Performance**: Slightly slower access, but better for large, dynamic projects.

        **Summary**:
        - Use Serialized Fields for small, static GUIs.
        - Use Dictionary for large, dynamic GUIs needing flexibility.
        */

        [SerializeField] private TMP_FontAsset defaultFont;
        public TMP_FontAsset DefaultFont => defaultFont;

        #region Home
        [Header("Home")]

        [SerializeField] private GUI_Settings guiSettings;
        public GUI_Settings GUISettings => guiSettings;

        [SerializeField] private GUIRating guiRating;
        public GUIRating GUIRating => guiRating;
        [SerializeField] private GUI_Language guiLanguage;
        public GUI_Language GUILanguage => guiLanguage;

        #endregion

        #region GamePlay
        [Header("GamePlay")]

        [SerializeField] private GUI_HUD guiHUD;
        public GUI_HUD GUIHUD => guiHUD;


        #endregion

        #region Shop
        [Header("Shop")]
        [SerializeField] private GUIShop guiShop;
        public GUIShop GUIShop => guiShop;

        #endregion


       

        #region Rewards

        [SerializeField] private GUIReward guiReward;
        public GUIReward GUIReward => guiReward;

        #endregion



        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}