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
    public partial class UGUIManager : Singleton<UGUIManager>
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
           
        }

        #endregion


        #region Sub GUI Event


        private void OnLanguageChanged()
        {
            
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
        }

        private void ButtonFindAllGUIBases()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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