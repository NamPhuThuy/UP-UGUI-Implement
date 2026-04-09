using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public class LocalCountPickerWindow : EditorWindow
{
    private int _total;
    private int _localCount;
    private Action<int> _onConfirm;
    private string _inputText = "0";

    public static void Show(int total, Action<int> onConfirm)
    {
        var window = GetWindow<LocalCountPickerWindow>("Local Sprite Count");
        window._total     = total;
        window._localCount = 0;
        window._onConfirm = onConfirm;
        window._inputText = "0";
        window.minSize    = new Vector2(340, 180);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("How many sprites ship locally?", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        EditorGUILayout.HelpBox(
            $"Total sprites found: {_total}\n" +
            $"Local (baked into APK): {_localCount}\n" +
            $"Remote only (AssetRef): {_total - _localCount}",
            MessageType.Info
        );

        EditorGUILayout.Space(8);

        // Slider + text field in sync
        EditorGUI.BeginChangeCheck();
        _localCount = EditorGUILayout.IntSlider("Local Count", _localCount, 0, _total);
        if (EditorGUI.EndChangeCheck())
            _inputText = _localCount.ToString();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Or type directly:", GUILayout.Width(100));
        _inputText = EditorGUILayout.TextField(_inputText);
        if (int.TryParse(_inputText, out int parsed))
            _localCount = Mathf.Clamp(parsed, 0, _total);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(12);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("All Local", GUILayout.Height(28)))
        {
            _localCount = _total;
            _inputText  = _total.ToString();
        }

        if (GUILayout.Button("All Remote", GUILayout.Height(28)))
        {
            _localCount = 0;
            _inputText  = "0";
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Confirm Import", GUILayout.Height(32)))
        {
            _onConfirm?.Invoke(_localCount);
            Close();
        }
        GUI.backgroundColor = Color.white;
    }
}
// ```
//
// ---
//
// ## What the Window Looks Like
// ```
// ┌─────────────────────────────────────┐
// │  How many sprites ship locally?     │
// │                                     │
// │  ℹ Total sprites found: 24          │
// │    Local (baked into APK): 6        │
// │    Remote only (AssetRef): 18       │
// │                                     │
// │  Local Count  [====|------]  6      │
// │  Or type directly: [ 6     ]        │
// │                                     │
// │  [All Local]        [All Remote]    │
// │                                     │
// │  [        Confirm Import        ]   │
// └─────────────────────────────────────┘ 
#endif