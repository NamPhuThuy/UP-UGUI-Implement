using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace NamPhuThuy.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class HeaderImageAutoFitter : MonoBehaviour
    {
        public enum AnchorPreset { TOP_CENTER, MIDDLE_CENTER, BOTTOM_CENTER }
        
        [Header("Flags")]
        [SerializeField] private AnchorPreset anchor = AnchorPreset.TOP_CENTER;
        
        [Header("Height Limit")]
        [Tooltip("Enable to limit maximum height based on screen/parent size")]
        [SerializeField] private bool useMaxHeightLimit = true;
        
        [Tooltip("Maximum height as percentage of parent height (0.3 = 30% of screen)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float maxHeightPercent = 0.3f;
        
        [Header("Stats")]
        [Tooltip("Extra pixels added to width and height.")]
        [SerializeField] private float padding = 0f;

        [Header("Components")]
        [SerializeField] private RectTransform parentRect;
        
        [SerializeField] private RectTransform selfRect;
        [SerializeField] private Image selfImage;

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            selfRect = GetComponent<RectTransform>();
            selfImage = GetComponent<Image>();

            if (parentRect == null)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null) parentRect = canvas.GetComponent<RectTransform>();
            }

            ApplyAnchor(anchor);
        }

        private void OnEnable()
        {
            Canvas.willRenderCanvases += Fit;
            Fit();
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= Fit;
        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            ApplyAnchor(anchor);
            Fit();
        }

        #endregion

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            Fit();
        }

        private void ApplyAnchor(AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.TOP_CENTER:
                    selfRect.anchorMin = selfRect.anchorMax = new Vector2(0.5f, 1f);
                    selfRect.pivot = new Vector2(0.5f, 1f);
                    break;
                case AnchorPreset.MIDDLE_CENTER:
                    selfRect.anchorMin = selfRect.anchorMax = new Vector2(0.5f, 0.5f);
                    selfRect.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.BOTTOM_CENTER:
                    selfRect.anchorMin = selfRect.anchorMax = new Vector2(0.5f, 0f);
                    selfRect.pivot = new Vector2(0.5f, 0f);
                    break;
            }
            selfRect.anchoredPosition = Vector2.zero;
        }

        private void Fit()
        {
            if (parentRect == null || selfImage == null || selfImage.sprite == null) return;

            Vector2 parentSize = parentRect.rect.size;
            var spr = selfImage.sprite;
            float spriteAspect = spr.rect.width / spr.rect.height;

            // ALWAYS fit width to parent width first
            float width = parentSize.x;
            
            // Calculate height based on sprite aspect ratio
            float height = width / spriteAspect;

            // Apply max height limit if enabled
            if (useMaxHeightLimit)
            {
                float maxHeight = Mathf.Clamp01(maxHeightPercent) * parentSize.y;
                
                if (height > maxHeight)
                {
                    height = maxHeight;
                    // Recalculate width to maintain aspect ratio
                    width = height * spriteAspect;
                }
            }

            // Apply padding
            selfRect.sizeDelta = new Vector2(width, height) + Vector2.one * padding;
        }
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(HeaderImageAutoFitter))]
    [CanEditMultipleObjects]
    public class HeaderImageAutoFitterEditor : Editor
    {
        private static bool s_AutoFit;

        private SerializedProperty _anchorProp;
        private SerializedProperty _useMaxHeightLimitProp;
        private SerializedProperty _maxHeightPercentProp;

        private void OnEnable()
        {
            _anchorProp = serializedObject.FindProperty("anchor");
            _useMaxHeightLimitProp = serializedObject.FindProperty("useMaxHeightLimit");
            _maxHeightPercentProp = serializedObject.FindProperty("maxHeightPercent");
            
            EditorApplication.update -= AutoFitTick;
            EditorApplication.update += AutoFitTick;
        }

        private void OnDisable()
        {
            EditorApplication.update -= AutoFitTick;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            // Show info box based on settings
            EditorGUILayout.Space();
            if (_useMaxHeightLimitProp.boolValue)
            {
                float percent = _maxHeightPercentProp.floatValue * 100f;
                EditorGUILayout.HelpBox(
                    $"Image will fit to full parent width, but height is limited to {percent:F0}% of parent height.\n" +
                    "If height exceeds limit, width will be reduced to maintain aspect ratio.",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Image will always fit to full parent width.\n" +
                    "Height is unlimited and calculated from sprite aspect ratio.\n" +
                    "⚠️ Warning: Tall sprites may exceed screen bounds!",
                    MessageType.Warning
                );
            }

            EditorGUILayout.Space();
            s_AutoFit = EditorGUILayout.Toggle("Auto Fit (Edit Mode)", s_AutoFit);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fit Now"))
            {
                CallFitOnTargets(targets);
            }
            if (GUILayout.Button("Apply Anchor + Fit"))
            {
                serializedObject.ApplyModifiedProperties();
                CallApplyAnchorOnTargets(targets, _anchorProp.intValue);
                CallFitOnTargets(targets);
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                // Re-apply anchor on change to reflect pivot/anchors instantly
                CallApplyAnchorOnTargets(targets, _anchorProp.intValue);
                CallFitOnTargets(targets);
            }
        }

        private void AutoFitTick()
        {
            if (!s_AutoFit || Application.isPlaying) return;

            foreach (var o in targets)
            {
                if (o == null) continue;
                CallFit(o);
            }
        }

        private static void CallApplyAnchorOnTargets(Object[] tgts, int anchorInt)
        {
            foreach (var o in tgts) CallApplyAnchor(o, anchorInt);
        }

        private static void CallFitOnTargets(Object[] tgts)
        {
            foreach (var o in tgts) CallFit(o);
        }

        private static void CallApplyAnchor(Object o, int anchorInt)
        {
            var comp = o as Component;
            if (comp == null) return;

            var type = comp.GetType();
            var method = type.GetMethod("ApplyAnchor", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null) return;

            var enumType = type.GetNestedType("AnchorPreset", BindingFlags.Public);
            var enumVal = Enum.ToObject(enumType, anchorInt);

            method.Invoke(comp, new[] { enumVal });
            MarkDirty(comp);
        }

        private static void CallFit(Object o)
        {
            var comp = o as Component;
            if (comp == null) return;

            var type = comp.GetType();
            var method = type.GetMethod("Fit", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null) return;

            method.Invoke(comp, null);
            MarkDirty(comp);
        }

        private static void MarkDirty(Component c)
        {
            if (c == null) return;
            EditorUtility.SetDirty(c);
            var rt = c.GetComponent<RectTransform>();
            if (rt != null) EditorUtility.SetDirty(rt);
            EditorSceneManager.MarkSceneDirty(c.gameObject.scene);
        }
    }

    #endif
}