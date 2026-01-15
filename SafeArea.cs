
using UnityEngine;

/* HOW TO USE
 Add this script to the parent object of all content in a UI-screen/popup to adjust the content in the safe-area 
 */

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace NamPhuThuy.UGUIImplement
{
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform rectTransform;

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            AdjustToSafeArea();
        }

        void OnEnable()
        {
            AdjustToSafeArea();
        }

        private void OnValidate()
        {
            // AdjustToSafeArea();
        }
        

        #endregion

        #region Public Methods

        public void EnsureFillComponents()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (canvas == null)
            {
                canvas = rectTransform.GetComponentInParent<Canvas>();
            }
        }

        #endregion
        
        public void AdjustToSafeArea()
        {

            EnsureFillComponents();
            Rect safeArea = Screen.safeArea;
       
            Vector2 safeAreaMin = safeArea.position;
            Vector2 safeAreaMax = safeArea.position + safeArea.size;

            Vector2 minAnchor = new Vector2(safeAreaMin.x / canvas.pixelRect.width, safeAreaMin.y / canvas.pixelRect.height);
            Vector2 maxAnchor = new Vector2(safeAreaMax.x / canvas.pixelRect.width, safeAreaMax.y / canvas.pixelRect.height);

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SafeArea))]
    public class SafeAreaEditor : Editor
    {
        private SafeArea _script;
        private void OnEnable()
        {
            _script = (SafeArea)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();

         

            EditorGUILayout.Space();

            // Add a prominent button to trigger the adjustment
            if (GUILayout.Button("Adjust to Safe Area Now", GUILayout.Height(30)))
            {
                _script.AdjustToSafeArea();
            }

            if (GUILayout.Button("Ensure Fill Components"))
            {
                _script.EnsureFillComponents();
            }

            EditorGUILayout.Space();

            // Display a helpful explanation for the user
            EditorGUILayout.HelpBox(
                "In the standard Editor Game view, Screen.safeArea returns the full screen size. " +
                "To test on devices with notches (like iPhones), use the 'Device Simulator' (Window > General > Device Simulator).",
                MessageType.Info);

            // Apply any changes made in the inspector
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
