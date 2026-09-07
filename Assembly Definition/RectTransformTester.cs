/*
Author: NamPhuThuy
Github: https://github.com/NamPhuThuy
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    public class RectTransformTester : MonoBehaviour
    {
        RectTransform rt;

        [Tooltip("Amount to move per click")] public float moveSpeed = 10f;

        [Tooltip("Amount to resize per click")]
        public float resizeSpeed = 10f;


        [Header("Components")] public RectTransform parentRT;
        public Image currentImage;

        [Header("Infor")] 
        public Vector2 anchoredPosition;
        public Vector2 position;
        public Vector2 localPosition;
        
        public Vector2 sizeDelta;
        public Vector2 finalSize;

        public Vector2 anchorMin;
        public Vector2 anchorMax;

        public Vector2 offsetMin;
        public Vector2 offsetMax;

        public Vector2 parentSize;
        public Vector2 anchorSize;
        public Vector2 pivot;

        #region MonoBehaviour Methods

        void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            anchoredPosition = rt.anchoredPosition;
            position = rt.position;
            localPosition = rt.localPosition;
            
            sizeDelta = rt.sizeDelta;
            finalSize = rt.rect.size;

            anchorMin = rt.anchorMin;
            anchorMax = rt.anchorMax;

            offsetMin = rt.offsetMin;
            offsetMax = rt.offsetMax;

            parentSize = rt.parent.transform.GetComponent<RectTransform>().rect.size;
            anchorSize = (anchorMax - anchorMin) * parentSize;
            pivot = rt.pivot;
        }

        #endregion

        public void Move(Vector2 direction)
        {
            if (rt == null) rt = GetComponent<RectTransform>();
            rt.anchoredPosition += direction * moveSpeed;
        }

        public void Resize(float direction)
        {
            if (rt == null) rt = GetComponent<RectTransform>();
            rt.sizeDelta += Vector2.one * (resizeSpeed * direction);
        }

        public void ChangePivot(Vector2 newPivot)
        {
            if (rt == null) rt = GetComponent<RectTransform>();
            rt.pivot = newPivot;
        }

        public void PrintInfo()
        {
            if (rt == null) rt = GetComponent<RectTransform>();


            Debug.Log(
                $"AnchoredPos: {anchoredPosition}\n" +
                $"Pos: {position}\n" +
                $"LocalPos: {localPosition}\n" +
                $"SizeDelta: {sizeDelta}\n" +
                $"AnchorMin/Max: {anchorMin} / {anchorMax}\n" +
                $"Pivot: {pivot}\n" +
                $"OffsetMin/Max: {offsetMin} / {offsetMax}" +
                $"\n" +
                $"ParentSize: {parentSize}\n" +
                $"AnchorSize = (anchorMax - anchorMin) * Parent Size: {anchorSize}\n" +
                $"FinalSize = Anchor Size + sizeDelta: {finalSize}\n"
            );
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RectTransformTester))]
    [CanEditMultipleObjects]
    public class RectTransformTesterEditor : Editor
    {
        private RectTransformTester script;

        private void OnEnable()
        {
            script = (RectTransformTester)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Manual Controls", EditorStyles.boldLabel);

            // Move
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Left")) ApplyAction(s => s.Move(Vector2.left), "Move Left");
            if (GUILayout.Button("Right")) ApplyAction(s => s.Move(Vector2.right), "Move Right");
            if (GUILayout.Button("Up")) ApplyAction(s => s.Move(Vector2.up), "Move Up");
            if (GUILayout.Button("Down")) ApplyAction(s => s.Move(Vector2.down), "Move Down");
            GUILayout.EndHorizontal();

            // Resize
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Grow")) ApplyAction(s => s.Resize(1f), "Resize Grow");
            if (GUILayout.Button("Shrink")) ApplyAction(s => s.Resize(-1f), "Resize Shrink");
            GUILayout.EndHorizontal();

            // Pivot
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pivot BL (0,0)"))
                ApplyAction(s => s.ChangePivot(new Vector2(0, 0)), "Pivot Bottom-Left");
            if (GUILayout.Button("Pivot Center (0.5,0.5)"))
                ApplyAction(s => s.ChangePivot(new Vector2(0.5f, 0.5f)), "Pivot Center");
            if (GUILayout.Button("Pivot TR (1,1)"))
                ApplyAction(s => s.ChangePivot(new Vector2(1, 1)), "Pivot Top-Right");
            GUILayout.EndHorizontal();

            // Info
            if (GUILayout.Button("Print Info"))
            {
                script.PrintInfo();
            }

            if (GUILayout.Button("Fit to Screen"))
            {
                script.currentImage.FitImageToRectTransformScreenSpaceOverlay(script.parentRT, ImageFitMode.COVER);
            }
        }

        private void ApplyAction(System.Action<RectTransformTester> action, string undoName)
        {
            foreach (var t in targets)
            {
                var s = (RectTransformTester)t;
                var rt = s.GetComponent<RectTransform>();
                if (rt != null)
                {
                    Undo.RecordObject(rt, undoName);
                    action(s);
                    EditorUtility.SetDirty(rt);
                }
            }
        }
    }
#endif
}