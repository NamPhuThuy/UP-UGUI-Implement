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

        [SerializeField] private GUIHome guiHome;
        public GUIHome GUIHome => guiHome;

        [SerializeField] private GUI_Settings guiSettings;
        public GUI_Settings GUISettings => guiSettings;

        [SerializeField] private GUIRating guiRating;
        public GUIRating GUIRating => guiRating;
        [SerializeField] private GUI_Language guiLanguage;
        public GUI_Language GUILanguage => guiLanguage;

        [SerializeField] private GUI_VIP guiVip;
        public GUI_VIP GUIVip => guiVip;

        #endregion

        #region GamePlay
        [Header("GamePlay")]

        [SerializeField] private GUI_HUD guiHUD;
        public GUI_HUD GUIHUD => guiHUD;

        [SerializeField] private GUIRevive guiRevive;
        public GUIRevive GUIRevive => guiRevive;

        [SerializeField] private GUILevelWin guiLevelWin;
        public GUILevelWin GUILevelWin => guiLevelWin;


        #endregion

        #region Shop
        [Header("Shop")]
        [SerializeField] private GUIShop guiShop;
        public GUIShop GUIShop => guiShop;

        [SerializeField] private GUI_NoAds guiNoAds;
        public GUI_NoAds GUINoAds => guiNoAds;

        #endregion

        #region Gallery

        [Header("Gallery")]

        [SerializeField] private GUIPictureDetails guiPictureDetails;
        public GUIPictureDetails GUIPictureDetails => guiPictureDetails;

        [SerializeField] private GUIPictureNew guiPictureNew;
        public GUIPictureNew GUIPictureNew => guiPictureNew;

        [SerializeField] private GUIGallery guiGallery;
        public GUIGallery GUIGallery => guiGallery;

        [SerializeField] private GUIGalleryPack guiGalleryPack;
        public GUIGalleryPack GUIGalleryPack => guiGalleryPack;

        [SerializeField] private GUIGalleryPackResults guiGalleryPackResults;
        public GUIGalleryPackResults GUIGalleryPackResults => guiGalleryPackResults;

        #endregion


        #region Notification
        [Header("Notification")]

        [SerializeField] private GUIPopupLoading guiPopupLoading;
        public GUIPopupLoading GUIPopupLoading => guiPopupLoading;

        [SerializeField] private GUINotEnoughCoin guiNotEnoughCoin;
        public GUINotEnoughCoin GUINotEnoughCoin => guiNotEnoughCoin;

        [SerializeField] private GUILoadingScreen guiLoadingScreen;
        public GUILoadingScreen GUILoadingScreen => guiLoadingScreen;

        [SerializeField] private GUI_Notification guiNotification;
        public GUI_Notification GUINotification => guiNotification;

        [SerializeField] private GUILevelDifficultyAlert gUILevelDifficultyAlert;
        public GUILevelDifficultyAlert GUILevelDifficultyAlert => gUILevelDifficultyAlert;
        #endregion

        #region Rewards

        [SerializeField] private GUIReward guiReward;
        public GUIReward GUIReward => guiReward;

        #endregion

        #region Tutorials

        [SerializeField] private GUI_Tutorial guiTutorial;
        public GUI_Tutorial GUITutorial => guiTutorial;

        #endregion

        #region Others

        [SerializeField] private GUI_Cheat guiCheat;
        public GUI_Cheat GUICheat => guiCheat;

        [SerializeField] private GUI_Favorite guiFavorite;
        public GUI_Favorite GUIFavorite => guiFavorite;

        #endregion


        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}