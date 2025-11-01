using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;


namespace NamPhuThuy.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class HeaderImageAutoFitter : MonoBehaviour
    {
        public enum AnchorPreset { TOP_CENTER, MIDDLE_CENTER, BOTTOM_CENTER }
        
        [Header("Flags")]
        [SerializeField] private AnchorPreset anchor = AnchorPreset.TOP_CENTER;
        
        [Header("Stats")]
        [Range(0.05f, 1f)]
        [SerializeField] private float maxHeightPercent = 0.3f;
        
        [Tooltip("Extra pixels added to width and height.")]
        [SerializeField] private float padding = 0f;

        [Header("Components")]
        [SerializeField] private RectTransform parentRect;
        
        [SerializeField] private RectTransform selfRect;
        [SerializeField] private Image selfImage;

        #region MonoBeviour Callbacks

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
            float maxH = Mathf.Clamp01(maxHeightPercent) * parentSize.y;

            var spr = selfImage.sprite;
            float spriteAspect = spr.rect.width / spr.rect.height;

            // Fit to parent width first
            float width = parentSize.x;
            float height = width / spriteAspect;

            // Clamp by max height while preserving aspect
            if (height > maxH)
            {
                height = maxH;
                width = height * spriteAspect;
            }

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

        private void OnEnable()
        {
            _anchorProp = serializedObject.FindProperty("anchor");
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

        [MenuItem("Tools/UI/Fit HeaderImageAutoFitter on Selection")]
        private static void FitSelection()
        {
            foreach (var go in Selection.gameObjects)
            {
                var comp = go.GetComponent<HeaderImageAutoFitter>();
                if (comp == null) continue;
                CallFit(comp);
            }
        }
    }

    #endif
}
