using System.Collections;
using DG.Tweening;
using Lean.Localization;
using MoreMountains.Tools;
using NamPhuThuy.AnimateWithScripts;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using NamPhuThuy.Lean_Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DebugLogger = NamPhuThuy.Common.DebugLogger;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class Booster : MonoBehaviour, MMEventListener<EBoosterDataUpdated>, MMEventListener<EATileIsChosen>
    {
        #region Private Serializable Fields

        public enum BoosterState
        {
            /// <summary>
            /// show a lock to cover the booster-icon
            /// </summary>
            LOCK = 0,

            /// <summary>
            /// show a coin panel, when player click the booster, it will check if there's enough coin or not
            /// if enough -> the booster will be activated
            /// if not enought -> a vfx say that: "not enough coin" will be shown
            /// </summary>
            NEED_COIN = 1,

            /// <summary>
            /// show a reward-ads icon, when player click the booster, reward ads will be shown
            /// after the reward ads is shown successfully, the booster will be activated
            /// </summary>
            NEED_RW_ADS = 2,

            /// <summary>
            /// number-icon will be show, player can click the booster to active it 
            /// </summary>
            AVAILABLE = 3, // Can be used

            /// <summary>
            /// there's a black mask will cover the booster icon, nothing happens when player click it
            /// Cant be used 
            /// </summary>
            UNAVAILABE = 4, // 

            /// <summary>
            /// When player stop interact with the game for a while
            /// the booster icon will flicker in a short time
            /// </summary>
            FLICKER = 5,
        }

        [Header("Flags")]
        [SerializeField] private BoosterType boosterType;
        [SerializeField] private BoosterState currentState;
        [SerializeField] private bool isActive = true;
        [SerializeField] private bool isCoolDown = false;
        [SerializeField] private bool isTapTooFast = false;

        public BoosterType BoosterType
        {
            get => boosterType;
            set => boosterType = value;
        }

        public bool IsTapTooFast
        {
            get => isTapTooFast;
            set => isTapTooFast = value;
        }

        [Header("Stats")]
        [SerializeField] private int currentPrice;

        [Header("Buttons")]
        [SerializeField] private Button boosterButton;

        [Header("AVAILABLE State")]
        [SerializeField] private Image boosterCountPanel;
        [SerializeField] private TextMeshProUGUI boosterCountText;
        [SerializeField] private Image reloadPanel;
        [SerializeField] private Image reloadIcon;

        [Header("LOCK State")]
        [SerializeField] private Image lockPanel;
        [SerializeField] private Image lockIcon;

        [Header("NEED COIN State")]
        [SerializeField] private Image needCoinImage;
        [SerializeField] private TextMeshProUGUI coinText;

        #endregion

        #region Private Methods

        private readonly float _cooldownSeconds = 1.5f;
        private Coroutine _cooldownCo;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            currentPrice = DataManager.Ins.BoosterData.GetBoosterRecord(boosterType).Price.GetCoinAmount();
            coinText.text = currentPrice.ToString();
        }

        private void OnEnable()
        {
            MMEventManager.RegistCurrentEvents(this);
            boosterButton.onClick.AddListener(OnClickBoosterButton);


            /*if (TutorialManager.Ins && TutorialManager.Ins.IsForcedLocked(boosterType))
            {
                SetState(BoosterState.LOCK);
                // DebugLogger.Log($"Booster.OnEnable() - Lock by tutor");
                return;
            }
            if (boosterType == BoosterType.UNDO && !GamePlayManager.Ins.GamePlayStateMachine.PlayLevelState.CanActiveUndo())
            {
                // DebugLogger.LogSimple(message:$"{boosterType} setState UNAVAILABE");
                SetState(BoosterState.UNAVAILABE);
                return;
            }*/

            DOVirtual.DelayedCall(0.1f, UpdateState);
        }



        private void OnDisable()
        {
            MMEventManager.UnregistCurrentEvents(this);
            boosterButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        public void UpdateState()
        {
            /*if (TutorialManager.Ins && TutorialManager.Ins.IsForcedLocked(boosterType))
            {
                SetState(BoosterState.LOCK);
                // DebugLogger.Log($"Booster.UpdateUI() - Lock by tutor");
                return;
            }

            if (TutorialManager.Ins && TutorialManager.Ins.IsForcedUnlocked(boosterType))
            {
                // Continue to decide state by amount
                // (Forced unlock just prevents “LOCK” state)
            }*/


            int boosterNum = DataManager.Ins.PInventoryData.GetBoosterNum(boosterType);
            // if (currentState == BoosterState.UNAVAILABE) return;

            if (boosterNum <= 0)
            {
                SetState(BoosterState.NEED_COIN);
            }
            else
            {
                SetState(BoosterState.AVAILABLE);
            }
        }

        private void UpdateUI()
        {

        }

        private void OnClickBoosterButton()
        {
            DebugLogger.Log(message:$"\nisActive: {isActive}, isCoolDown: {isCoolDown}, currentState: {currentState}");
            if (!isActive)
            {
                ShowPopupText(LeanLocalizedConst.CANT_CLICK);
                return;
            }

            if (isTapTooFast)
            {
                ShowPopupText(LeanLocalizedConst.READYING);
                return;
            }

            // Debug.LogError($"Booster.OnClickBoosterButton() - is Active");
            /*if (TutorialManager.Ins && TutorialManager.Ins.IsForcedLocked(boosterType))
            {
                ShowBoosterIsLockedVFX();
                // DebugLogger.Log($"Booster.OnClickBoosterButton() - Booster is locked by Tutorial");
                return;
            }*/

            // DebugLogger.Log($"Booster.OnClickBoosterButton() - currentState: {currentState}");
            switch (currentState)
            {
                case BoosterState.LOCK:
                    ShowPopupText(LeanLocalizedConst.BOOSTER_IS_LOCKED);
                    break;
                case BoosterState.AVAILABLE:
                    int tmp = DataManager.Ins.PInventoryData.GetBoosterNum(boosterType);
                    if (tmp > 0)
                    {
                        ActiveBooster();
                    }
                    break;
                case BoosterState.NEED_COIN:
                    if (DataManager.Ins.PInventoryData.TrySpendCoins(currentPrice))
                    {
                        ActiveBooster();
                    }
                    else
                    {
                        // GUIManager.Ins.ShowGUI(GUIManager.Ins.GUINotEnoughCoin);
                        MMEventManager.TriggerEvent(new EGUIShow_Fire()
                        {
                            guiId = GUIBase.GUIId.GUI_NOT_ENOUGH_COIN
                        });
                    }
                    break;
                case BoosterState.UNAVAILABE:
                    ShowPopupText(LeanLocalizedConst.BOOSTER_IS_LOCKED);
                    break;
            }

            void ActiveBooster()
            {
                DebugLogger.Log(message:$"Active type {boosterType}");
                MMEventManager.TriggerEvent(new EBoosterActive_Fire(boosterType));
                CheckUndoBoosterCanActive();
            }
        }

        public void CheckUndoBoosterCanActive()
        {
            // DebugLogger.Log($"Booster.CheckUndoBoosterCanActive()");
            if (boosterType != BoosterType.UNDO) return;
            /*if (TutorialManager.Ins && TutorialManager.Ins.IsForcedLocked(boosterType))
                return;*/

            /*if (!GamePlayManager.Ins.GamePlayStateMachine.PlayLevelState.CanActiveUndo())
            {
                // DebugLogger.LogSimple(message:$"{boosterType} - setState UNAVAILABE");
                SetState(BoosterState.UNAVAILABE);
            }*/
        }

        private IEnumerator IECheckUndoBoosterCanActive()
        {
            yield return YieldHelper.WaitForSeconds(0.35f);
            CheckUndoBoosterCanActive();
        }

        private IEnumerator IECoolDownBooster()
        {

            ValidateReloadIcon(reloadIcon);

            DeadActiveBoosterFlag();
            isCoolDown = true;
            if (reloadPanel) reloadPanel.gameObject.SetActive(true);
            if (reloadIcon) reloadIcon.gameObject.SetActive(true);

            // Fill from 1 -> 0 over cooldownSeconds (unscaled so it still runs if Time.timeScale=0)
            float t = 0f;
            if (reloadIcon) reloadIcon.fillAmount = 1f;

            while (t < _cooldownSeconds)
            {
                t += Time.unscaledDeltaTime;
                if (reloadIcon) reloadIcon.fillAmount = Mathf.Lerp(1f, 0f, Mathf.Clamp01(t / _cooldownSeconds));
                yield return null;

                if (reloadIcon.fillAmount <= 0.05f)
                {
                    reloadIcon.fillAmount = 0f;
                    break;
                }
            }

            // Cooldown finished — allow interaction again and restore state
            ActiveBoosterFlag();
            isCoolDown = false;
            reloadPanel.gameObject.SetActive(false);

            // Decide final state based on current quantities/conditions
            UpdateState();
            CheckUndoBoosterCanActive();

            _cooldownCo = null;

            void ValidateReloadIcon(Image _reloadIcon)
            {
                if (_reloadIcon == null) return;
                if (_reloadIcon.type == Image.Type.Filled) return;

                _reloadIcon.type = Image.Type.Filled;
            }
        }

        private void ShowPopupText(string message)
        {
            var args = new ToastArgs
            {
                Message = LeanLocalization.GetTranslationText(message),
                CustomAnchoredPos = AnimationConst.UPPER_ANCHORED_POS,
                TextFont = UGUIManager.Ins.DefaultFont,
                TextColor = Color.white,
                customDuration = 0.5f,
            };
            AnimationManager.Ins.Play(args);
        }

        #endregion

        #region Public Methods

        public void SetState(BoosterState nextState)
        {
            currentState = nextState;
            // DebugLogger.LogSimple(message:$"{boosterType} - {nextState}");
            switch (currentState)
            {
                case BoosterState.LOCK:
                    DeadActiveBoosterFlag();

                    needCoinImage.gameObject.SetActive(false);
                    boosterCountPanel.gameObject.SetActive(false);
                    lockPanel.gameObject.SetActive(true);
                    lockIcon.gameObject.SetActive(true);
                    break;

                case BoosterState.NEED_COIN:
                    lockIcon.gameObject.SetActive(false);
                    boosterCountPanel.gameObject.SetActive(false);
                    lockPanel.gameObject.SetActive(false);
                    needCoinImage.gameObject.SetActive(true);
                    break;

                case BoosterState.NEED_RW_ADS:
                    DeadActiveBoosterFlag();
                    break;

                case BoosterState.AVAILABLE:
                    lockPanel.gameObject.SetActive(false);
                    lockIcon.gameObject.SetActive(false);

                    if (!isCoolDown)
                    {
                        ActiveBoosterFlag();
                    }

                    needCoinImage.gameObject.SetActive(false);

                    boosterCountPanel.gameObject.SetActive(true);
                    boosterCountText.text = DataManager.Ins.PInventoryData.GetBoosterNum(boosterType).ToString();
                    break;

                case BoosterState.UNAVAILABE:
                    DeadActiveBoosterFlag();

                    lockIcon.gameObject.SetActive(false);
                    boosterCountPanel.gameObject.SetActive(false);
                    needCoinImage.gameObject.SetActive(false);
                    lockPanel.gameObject.SetActive(true);
                    break;

                case BoosterState.FLICKER:
                    break;

            }
        }

        public void StartBoosterCooldown()
        {
            if (_cooldownCo != null) StopCoroutine(_cooldownCo);
            _cooldownCo = StartCoroutine(IECoolDownBooster());
        }

        public void FinishIECoolDown()
        {
            if (_cooldownCo != null)
                StopCoroutine(_cooldownCo);

            reloadIcon.fillAmount = 0f;
            ActiveBoosterFlag();
            isCoolDown = false;

            try
            {
                reloadPanel.gameObject.SetActive(false);
            }
            catch (System.Exception)
            {

            }
        }


        /// <summary>
        /// Use when the game is about to victory
        /// </summary>
        public void TurnOnTheLock()
        {
            try
            {
                reloadPanel.gameObject.SetActive(true);
                reloadIcon.fillAmount = 1f;
                DeadActiveBoosterFlag();
            }
            catch (System.Exception)
            {

            }
        }

        public void ActiveBoosterFlag()
        {
            isActive = true;
            // DebugLogger.LogSimple(message:$"{boosterType}, {isActive}");
        }

        public void DeadActiveBoosterFlag()
        {

            isActive = false;
            // DebugLogger.LogSimple(message:$"{boosterType}, {isActive}");
        }


        #endregion

        #region Events Listen

        public void OnMMEvent(EBoosterDataUpdated eventArgs)
        {
            if (eventArgs.BoosterType == boosterType)
            {
                UpdateState();
            }
        }

        public void OnMMEvent(EATileIsChosen eventArgs)
        {
            if (boosterType != BoosterType.UNDO) return;
            if (isCoolDown) return;

            UpdateState();
            ActiveBoosterFlag();
            CheckUndoBoosterCanActive();
        }

        /*public void OnMMEvent(EPairMatched eventArgs)
        {
            StartCoroutine(IECheckUndoBoosterCanActive());
        }*/

        #endregion



    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Booster)), CanEditMultipleObjects]
    public class BoosterEditor : Editor
    {
        private SerializedProperty _boosterType;
        private SerializedProperty _currentState;
        private SerializedProperty _isActive;

        private void OnEnable()
        {
            /*_boosterType = serializedObject.FindProperty("boosterType");
            _currentState = serializedObject.FindProperty("currentState");
            _isActive     = serializedObject.FindProperty("isActive");*/
        }

        public override void OnInspectorGUI()
        {
            // draw your default serialized fields first
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            // spacing & header
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Booster State Controls", EditorStyles.boldLabel);

            // dropdown to pick a state
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            // EditorGUILayout.PropertyField(_currentState, new GUIContent("Preview State"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                // optionally do a live preview of state change when dropdown changes:
                foreach (var t in targets)
                {
                    var b = (Booster)t;
                    // record for Undo
                    Undo.RecordObject(b, "Preview Booster State");
                    b.SetState((Booster.BoosterState)_currentState.enumValueIndex);
                    EditorUtility.SetDirty(b);
                }
                MarkSceneDirtyIfNeeded();
            }

            EditorGUILayout.Space(6);

            // Apply button (useful if you don’t want live preview)
            if (GUILayout.Button("Apply SetState()"))
            {
                foreach (var t in targets)
                {
                    var b = (Booster)t;
                    Undo.RecordObject(b, "Apply Booster State");
                    b.SetState((Booster.BoosterState)_currentState.enumValueIndex);
                    EditorUtility.SetDirty(b);
                }
                MarkSceneDirtyIfNeeded();
            }

            EditorGUILayout.Space(6);

            // quick buttons: one per state
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("LOCK")) ApplyStateToTargets(Booster.BoosterState.LOCK);
                if (GUILayout.Button("NEED_COIN")) ApplyStateToTargets(Booster.BoosterState.NEED_COIN);
                if (GUILayout.Button("NEED_RW_ADS")) ApplyStateToTargets(Booster.BoosterState.NEED_RW_ADS);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("AVAILABLE")) ApplyStateToTargets(Booster.BoosterState.AVAILABLE);
                if (GUILayout.Button("UNAVAILABLE")) ApplyStateToTargets(Booster.BoosterState.UNAVAILABE);
                if (GUILayout.Button("FLICKER")) ApplyStateToTargets(Booster.BoosterState.FLICKER);
            }
        }

        private void ApplyStateToTargets(Booster.BoosterState state)
        {
            serializedObject.Update();
            _currentState.enumValueIndex = (int)state;
            serializedObject.ApplyModifiedProperties();

            foreach (var t in targets)
            {
                var b = (Booster)t;
                Undo.RecordObject(b, $"Set Booster State: {state}");
                b.SetState(state);
                EditorUtility.SetDirty(b);
            }
            MarkSceneDirtyIfNeeded();
        }

        private static void MarkSceneDirtyIfNeeded()
        {
            if (!Application.isPlaying)
            {
                var scene = EditorSceneManager.GetActiveScene();
                if (scene.IsValid()) EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }
#endif
}