using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MoreMountains.Tools;
using NamPhuThuy.Common;
using UnityEngine;
using System.Linq;

#if USE_LEAN_LOCALIZATION
using Lean.Localization;
#endif

using NamPhuThuy.PuzzleTutorial;


#if USE_LEAN_LOCALIZATION
using NamPhuThuy.Lean_Localization;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// ReSharper disable once CheckNamespace
namespace NamPhuThuy.UGUIImplement
{
    public partial class UGUIManager : Common.Singleton<UGUIManager>, MMEventListener<ELevelLoaded>, MMEventListener<ELevelLoad_Failed>, MMEventListener<ELevelLoad_OutRange>, MMEventListener<ELevelFinished>, MMEventListener<EBoosterActive_Fire>, MMEventListener<EBoosterDataUpdated>, MMEventListener<EATileIsChosen>, MMEventListener<ETutorialStarted>,MMEventListener<ETutorialFinished>, MMEventListener<EPictureRemoteDownloaded>, MMEventListener<ETutorialStepStarted>, MMEventListener<EGUIShow_Fire> /*, MMEventListener<EActiveNoAds>, MMEventListener<EPictureUnlocked>*/
    {

        [SerializeField] private List<GUIBase> guiList = new List<GUIBase>();

        public List<GUIBase> GUIList => guiList;

        private Dictionary<GUIBase.GUIId, GUIBase> _dictGUIIdToGUIBase;
        
        [SerializeField] public GUIBase[] popupPrefabs;

        [Header("Flags")]
        [SerializeField] private bool isShowingGUI;

        [Header("Stats")]
        [SerializeField] private int visibleGUICount;

        [Header("Components")]
        public CoinPanel defaultCoinPanel;
        public CoinPanel currentCoinPanel;
        public List<CoinPanel> cachedCoinPanels;

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();
            RebuildGUIDict();
            
            MMEventManager.RegistCurrentEvents(this);

            GUI_Language.LanguageChanged += OnLanguageChanged;
        }
        
        private void Start()
        {
            /*foreach (var gui in guiList)
            {
                if (gui == null) continue;
                gui.OnShow += OnGUIShow;
                gui.OnHide += OnGUIHide;
            }*/
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MMEventManager.UnregistCurrentEvents(this);
            
            GUI_Language.LanguageChanged -= OnLanguageChanged;
        }

        
        private void Init()
        {
            for (int i = 0; i < popupPrefabs.Length; i++)
            {
                Instantiate(popupPrefabs[i], transform);
            }

            FindAllGUIBaseRuntime();

            HideAllGUIs();
            ShowGUI(guiHome);
        }

        private void OnEnable()
        {
           
            if (guiPictureNew != null) GUIPictureNew.ChangeLikeState += OnChangeLikeState;
            if (guiPictureDetails != null) GUIPictureDetails.ChangeLikeState += OnChangeLikeState;
        }

        private void OnDisable()
        {
            
            if (guiPictureNew != null) GUIPictureNew.ChangeLikeState -= OnChangeLikeState;
            if (guiPictureDetails != null) GUIPictureDetails.ChangeLikeState -= OnChangeLikeState;
        }

   
        #endregion

        #region Public Methods

        private void RebuildGUIDict()
        {
            DebugLogger.Log();
            _dictGUIIdToGUIBase = new Dictionary<GUIBase.GUIId, GUIBase>();
            foreach (GUIBase gui in guiList)
            {
                if (gui != null)
                    _dictGUIIdToGUIBase[gui.GuiId] = gui;
            }
            
            DebugLogger.LogDictionary(_dictGUIIdToGUIBase, title: $"_dictGUIIdToGUIBase");
        }

        public GUIBase GetGUI(GUIBase.GUIId id)
        {
            _dictGUIIdToGUIBase.TryGetValue(id, out GUIBase gui);
            return gui;
        }

        public void FindAllGUIBaseRuntime()
        {
            guiList.Clear();

            // Add all GUIBase components in children
            GUIBase[] guiBases = GetComponentsInChildren<GUIBase>(true);
            guiList.AddRange(guiBases);

            // Use reflection to assign single GUIBase fields
            var fields = GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(GUIBase).IsAssignableFrom(field.FieldType))
                {
                    var component = GetComponentInChildren(field.FieldType, true);
                    if (component != null)
                    {
                        field.SetValue(this, component);
                    }
                }
            }

            RebuildGUIDict();
        }

#if UNITY_EDITOR
        public void FindAllGUIBase()
        {
            guiList.Clear();
            GUIBase[] guiBases = GetComponentsInChildren<GUIBase>(true);
            guiList.AddRange(guiBases);

            var fields = GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(GUIBase).IsAssignableFrom(field.FieldType))
                {
                    var component = GetComponentInChildren(field.FieldType, true);
                    if (component != null)
                    {
                        field.SetValue(this, component);
                    }
                }
            }
        }
#endif

        public CoinPanel AutoFindResourceDisplay()
        {
            CoinPanel resCoinPanel = currentCoinPanel;

            if (resCoinPanel == null)
            {
                resCoinPanel = defaultCoinPanel;
            }
            else if (!resCoinPanel.gameObject.activeInHierarchy)
            {
                if (cachedCoinPanels.Any(item => item.gameObject.activeInHierarchy))
                {
                    resCoinPanel = cachedCoinPanels.First(item => item.gameObject.activeInHierarchy);
                }
                else
                {
                    resCoinPanel = defaultCoinPanel;
                }
            }

            return resCoinPanel;
        }

        #endregion

        #region Sub-GUI Control

        public void ShowGUI(GUIBase guiShow, float delay = 0f, params object[] parameters)
        {
            DebugLogger.Log(message:$"Show: {guiShow.name}");
            StartCoroutine(IE_ShowGUI(guiShow, delay, parameters));
        }

        private IEnumerator IE_ShowGUI(GUIBase guiShow, float f = 0f, params object[] parameters)
        {
            yield return YieldHelper.WaitForSeconds(f);
            guiShow.Show(parameters);
        }
        
        public void HideGUI(GUIBase guiHide, float delay = 0f, params object[] parameters)
        {
            DebugLogger.Log(message:$"Hide: {guiHide.name}");
            StartCoroutine(IE_HideGUI(guiHide, delay, parameters));
        }
        
        private IEnumerator IE_HideGUI(GUIBase guiHide, float f = 0f, params object[] parameters)
        {
            yield return YieldHelper.WaitForSeconds(f);
            guiHide.Hide(parameters);
        }
        
        public void HideGUIFast(GUIBase guiHide, float delay = 0f, params object[] parameters)
        {
            DebugLogger.Log(message:$"Hide: {guiHide.name}");
            StartCoroutine(IE_HideGUIFast(guiHide, delay, parameters));
        }
        
        private IEnumerator IE_HideGUIFast(GUIBase guiHide, float f = 0f, params object[] parameters)
        {
            yield return YieldHelper.WaitForSeconds(f);
            guiHide.HideFast(parameters);
        }


        #endregion

        #region Private Methods

        // ReSharper disable once InconsistentNaming
        private void HideAllGUIs()
        {
            // DebugLogger.Log();
            foreach (GUIBase gui in guiList)
            {
                
                if (gui == null) continue;
                if (gui.gameObject.activeSelf)
                {
                    // DebugLogger.Log(message:$"Hide {gui.name}");
                    gui.Hide();
                }
            }
        }

        private void OnGUIShow()
        {
            visibleGUICount++;
#if USE_AUDIO
            AudioManager.Ins.Play(AudioEnum.SFX_POP_UP_SHOW);
#endif
        }

        private void OnGUIHide(GUIBase.GUIId guiId)
        {
            MMEventManager.TriggerEvent(new EGUIHidden()
            {
                guiId = guiId
            });
            visibleGUICount = Mathf.Max(0, visibleGUICount - 1);
        }

        public void CacheAllCoinPanels()
        {
            cachedCoinPanels = transform.GetComponentsFromAllChildren<CoinPanel>().ToList();
        }

        #endregion

        #region Events Listen

        public void OnMMEvent(ELevelLoaded eventType)
        {
            DebugLogger.Log();
            HideAllGUIs();
            ShowGUI(GUIHUD);

            GUIHUD.EnableInteract();

            foreach (Booster booster in GUIHUD.BoosterList)
            {
                booster.FinishIECoolDown();
            }
        }

        public void OnMMEvent(ELevelLoad_Failed eventType)
        {
            DebugLogger.Log();
            HideAllGUIs();
            ShowGUI(guiHome);
            ShowGUI(guiNotification, 0f, LeanLocalization.GetTranslationText(LeanLocalizedConst.CHECK_INTERNET), LeanLocalization.GetTranslationText(LeanLocalizedConst.CANT_LOAD_LEVEL));
        }
        
        public void OnMMEvent(ELevelLoad_OutRange eventArgs)
        {
            HideAllGUIs();
            ShowGUI(guiHome);
            ShowGUI(guiNotification, 0f, LeanLocalization.GetTranslationText(LeanLocalizedConst.YOURE_FINISHED), LeanLocalization.GetTranslationText(LeanLocalizedConst.NEW_LEVELS_ARE_COMING_SOON));
        }

        public void OnMMEvent(ELevelFinished eventType)
        {
            if (eventType.IsWin)
            {
                HideAllGUIs();
                ShowGUI(GUILevelWin);
            }
            else
            {
                ShowGUI(GUIRevive);
            }
        }

        public void OnMMEvent(EBoosterActive_Fire eventType)
        {
            foreach (Booster booster in GUIHUD.BoosterList)
            {
                booster.StartBoosterCooldown();
            }
        }

        public void OnMMEvent(EBoosterDataUpdated eventType)
        {
            // GUIHUD.UpdateUIBoosters(eventType.BoosterType);
        }
        
        public void OnMMEvent(EATileIsChosen eventType)
        {
            guiHUD.TurnOffTutorialImage();
        }
        
        public void OnMMEvent(EPictureRemoteDownloaded eventArgs)
        {
            DebugLogger.Log(message: $"Receive EPictureRemoteDownloaded");
            foreach (ElementGallery element in guiGallery.ScrollViewGallery.ElementList)
            {
                if (eventArgs.pictureId == element.pictureId)
                {
                    element.HidePlaceholder();
                    element.contentImage.sprite = eventArgs.girlSprite;
                    element.FitImageToRectTransform();
                    break;
                }
            }
        }

        /*public void OnMMEvent(EPictureUnlocked eventType)
        {
            guiGallery.ScrollViewGallery.AddElement(eventType.pictureId);

            // guiLoadingScreen.LoadingImage.sprite = DataManager.Ins.PictureDatas.allPictureDatas[eventType.pictureId].mainImage;
            // guiLoadingScreen.FitImage();
            guiLoadingScreen.UpdateBackImage(eventType.pictureId);
        }

        public void OnMMEvent(EActiveNoAds eventType)
        {
            guiHome.ValidateNoAdsButton();
            guiHUD.ValidateNoAdsButton();
            guiNoAds.Hide();
        }*/
        
        public void OnMMEvent(ETutorialStarted eventArgs)
        {
            DebugLogger.Log(message:$"isForceFollow: {eventArgs.isForceFollow}");
            if (eventArgs.isForceFollow)
            {
                guiHUD.DisableInteract();
                guiHUD.LockBoosters();
            }
        }
        
        public void OnMMEvent(ETutorialFinished eventArgs)
        {
            // guiHUD.EnableInteract();
            guiHUD.ToggleIsHandTutShowing(false);
        }
        
        public void OnMMEvent(ETutorialStepStarted eventArgs)
        {
            if (eventArgs.StepRecord.Type == TutorialStepType.HAND_POINT_TARGET_WAIT_HOLD
                || eventArgs.StepRecord.Type == TutorialStepType.HAND_POINT_BOOSTER_WAIT_CLICK
                || eventArgs.StepRecord.Type == TutorialStepType.HAND_POINT_FOR_BOOSTER
                || eventArgs.StepRecord.Type == TutorialStepType.HAND_POINT_TARGET_WAIT_CLICK)
            {
                // guiHUD.DisableInteract();
                guiHUD.ToggleIsHandTutShowing(true);
            }

            if (eventArgs.StepRecord.TargetTagOrId == "0")
            {
                guiHUD.LockBooster(1);
                // guiHUD.LockBooster(2);
            }
            else if (eventArgs.StepRecord.TargetTagOrId == "1")
            {
                guiHUD.LockBooster(0);
                // guiHUD.LockBooster(2);
            }
            else if (eventArgs.StepRecord.TargetTagOrId == "2")
            {
                guiHUD.LockBooster(0);
                guiHUD.LockBooster(1);
            }
        }
        
        public void OnMMEvent(EGUIShow_Fire eventArgs)
        {
            GUIBase gui = GetGUI(eventArgs.guiId);
            if (gui != null)
                ShowGUI(gui, eventArgs.delay, eventArgs.parameters);
        }

        public void OnLanguageChanged()
        {
            DebugLogger.Log();
            guiHUD.UpdateLocalizedTextWithParams();
            guiHome.UpdateLocalizedTextWithParams();
            guiLevelWin.UpdateLocalizedTextWithParams();
        }
        
      
        
        #endregion

        #region Sub GUI Event

        private void OnChangeLikeState()
        {
            GUIPictureDetails.SetStateReactButtons();
            GUIPictureNew.SetStateForButton();
            GUIGallery.OnChangeLikeState();
        }

       

        #endregion


        
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(UGUIManager))]
    public class GUIManagerEditor : Editor
    {
        private UGUIManager _uguiManager;
        // private FieldInfo[] _guiFields;
        
        private void OnEnable()
        {
            // _guiFields = typeof(GUIManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _uguiManager = (UGUIManager)target;

            ButtonFindAllGUIBases();
            ButtonFindAllCoinPanels();
        }

        private void ButtonFindAllGUIBases()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Find All GUIBase", GUILayout.Width(InspectorConst.BUTTON_WIDTH_LARGE)))
            {
                _uguiManager.FindAllGUIBase();
                EditorSceneManager.MarkSceneDirty(_uguiManager.gameObject.scene);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void ButtonFindAllCoinPanels()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Find All Coin Panels", GUILayout.Width(InspectorConst.BUTTON_WIDTH_LARGE)))
            {
                _uguiManager.CacheAllCoinPanels();
                EditorSceneManager.MarkSceneDirty(_uguiManager.gameObject.scene);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static List<GUIBase> LoadAllGUIBaseFromFolder(string folderPath)
        {
            List<GUIBase> guiList = new List<GUIBase>();

            // Find all prefab GUIDs in the folder
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    GUIBase guiBase = prefab.GetComponent<GUIBase>();
                    if (guiBase != null)
                    {
                        guiList.Add(guiBase);
                    }
                }
            }

            return guiList;
        }

        
        /*private void ButtonShowAndHide()
        {
            // Iterate through all serialized GUI fields
            var fields = typeof(GUIManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(GUIBase).IsAssignableFrom(field.FieldType))
                {
                    GUIBase gui = field.GetValue(_guiManager) as GUIBase;
                    if (gui != null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(field.Name, GUILayout.Width(150));

                        if (GUILayout.Button("Show", GUILayout.Width(InspectorConst.BUTTON_WIDTH_SMALL)))
                        {
                            gui.Show();
                            EditorUtility.SetDirty(_guiManager);
                        }

                        if (GUILayout.Button("Hide", GUILayout.Width(InspectorConst.BUTTON_WIDTH_SMALL)))
                        {
                            gui.Hide();
                            EditorUtility.SetDirty(_guiManager);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }
        }*/
    }
#endif
}