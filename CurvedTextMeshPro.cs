using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIAdapter
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurvedTextMeshPro : MonoBehaviour
    {
        #region Enums
        
        public enum CurveAxis
        {
            VERTICAL = 0,
            HORIZONTAL = 1,
            BOTH = 2
        }
        
        public enum UpdateMode
        {
            ON_TEXT_CHANGE = 0,   // Only update when text changes
            EVERY_FRAME = 1,     // Update every frame (for animations)
            MANUAL = 2          // Only update via ApplyCurve() call
        }
        
        #endregion
        
        #region Serialized Fields
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI textMeshPro;
        
        [Header("Curve Settings")]
        [SerializeField] private AnimationCurve verticalCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve horizontalCurve = AnimationCurve.Linear(0, 0, 1, 0);
        [SerializeField] private CurveAxis curveAxis = CurveAxis.VERTICAL;
        
        [Header("Intensity")]
        [SerializeField] [Range(0f, 100f)] private float curveHeight = 10f;
        [SerializeField] [Range(0f, 100f)] private float curveWidth = 0f;
        
        [Header("Advanced Settings")]
        [SerializeField] private UpdateMode updateMode = UpdateMode.ON_TEXT_CHANGE;
        [SerializeField] private bool useCharacterWidth = true; // Better curve distribution
        [SerializeField] private bool preserveAspectRatio = true;
        [SerializeField] private bool animateCurve = false;
        [SerializeField] private float animationSpeed = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion
        
        #region Private Fields
        
        private string _lastText;
        private float _animationTime;
        private bool _isDirty = true;
        
        #endregion
        
        #region Properties
        
        public TMP_Text TextMeshPro
        {
            get
            {
                if (textMeshPro == null)
                {
                    textMeshPro = GetComponent<TextMeshProUGUI>();
                }
                return textMeshPro;
            }
        }
        
        public float CurveHeight
        {
            get => curveHeight;
            set
            {
                if (!Mathf.Approximately(curveHeight, value))
                {
                    curveHeight = value;
                    _isDirty = true;
                }
            }
        }
        
        public float CurveWidth
        {
            get => curveWidth;
            set
            {
                if (!Mathf.Approximately(curveWidth, value))
                {
                    curveWidth = value;
                    _isDirty = true;
                }
            }
        }
        
        #endregion
        
        #region Unity Callbacks
        
        private void Awake()
        {
            if (textMeshPro == null)
            {
                textMeshPro = GetComponent<TextMeshProUGUI>();
            }
            
            if (textMeshPro != null)
            {
                _lastText = textMeshPro.text;
            }
        }
        
        private void OnEnable()
        {
            _isDirty = true;
            ApplyCurve();
            
            // Subscribe to text change events
            if (textMeshPro != null)
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            }
        }
        
        private void OnDisable()
        {
            if (textMeshPro != null)
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
            }
        }
        
        private void Update()
        {
            if (updateMode == UpdateMode.EVERY_FRAME || (animateCurve && updateMode != UpdateMode.MANUAL))
            {
                _isDirty = true;
            }
            
            if (updateMode != UpdateMode.MANUAL)
            {
                // Check if text has changed
                if (textMeshPro != null && _lastText != textMeshPro.text)
                {
                    _lastText = textMeshPro.text;
                    _isDirty = true;
                }
            }
            
            if (_isDirty)
            {
                ApplyCurve();
            }
            
            if (animateCurve)
            {
                _animationTime += Time.deltaTime * animationSpeed;
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                _isDirty = true;
                ApplyCurve();
                // Delay call to avoid issues in edit mode
                /*EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                    
                        ApplyCurve();
                    }
                };*/
            }
        }
#endif
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Manually apply the curve to the text
        /// </summary>
        public void ApplyCurve()
        {
            if (TextMeshPro == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("TextMeshPro component not found!", this);
                return;
            }
            
            // Force mesh update
            TextMeshPro.ForceMeshUpdate(true);
            
            var textInfo = TextMeshPro.textInfo;
            
            if (textInfo == null || textInfo.characterCount == 0)
            {
                _isDirty = false;
                return;
            }
            
            // Calculate total text width if using character width distribution
            float totalWidth = 0f;
            if (useCharacterWidth)
            {
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (charInfo.isVisible)
                    {
                        totalWidth += charInfo.xAdvance;
                    }
                }
            }
            
            float accumulatedWidth = 0f;
            
            // Apply curve to each character
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;
                
                int matIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                
                Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;
                
                // Calculate normalized position (0 to 1)
                float t;
                if (useCharacterWidth && totalWidth > 0)
                {
                    t = accumulatedWidth / totalWidth;
                    accumulatedWidth += charInfo.xAdvance;
                }
                else
                {
                    t = textInfo.characterCount > 1 ? (float)i / (textInfo.characterCount - 1) : 0.5f;
                }
                
                // Add animation if enabled
                float animatedT = t;
                if (animateCurve)
                {
                    animatedT = (t + _animationTime) % 1f;
                }
                
                // Calculate offsets based on curve axis
                Vector3 offset = Vector3.zero;
                
                if (curveAxis == CurveAxis.VERTICAL || curveAxis == CurveAxis.BOTH)
                {
                    float offsetY = verticalCurve.Evaluate(animatedT) * curveHeight;
                    offset.y = offsetY;
                }
                
                if (curveAxis == CurveAxis.HORIZONTAL || curveAxis == CurveAxis.BOTH)
                {
                    float offsetX = horizontalCurve.Evaluate(animatedT) * curveWidth;
                    offset.x = offsetX;
                }
                
                // Apply offset to all 4 vertices of the character
                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
                
                // Optional: Rotate characters to follow curve tangent
                if (preserveAspectRatio && (curveAxis == CurveAxis.VERTICAL || curveAxis == CurveAxis.BOTH))
                {
                    ApplyCharacterRotation(vertices, vertexIndex, animatedT);
                }
            }
            
            // Apply mesh changes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                if (meshInfo.mesh != null)
                {
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    TextMeshPro.UpdateGeometry(meshInfo.mesh, i);
                }
            }
            
            _isDirty = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"Applied curve to {textInfo.characterCount} characters", this);
            }
        }
        
        /// <summary>
        /// Reset the curve to default linear state
        /// </summary>
        public void ResetCurve()
        {
            verticalCurve = AnimationCurve.Linear(0, 0, 1, 1);
            horizontalCurve = AnimationCurve.Linear(0, 0, 1, 0);
            curveHeight = 0f;
            curveWidth = 0f;
            _isDirty = true;
            ApplyCurve();
        }
        
        /// <summary>
        /// Set a preset arc curve
        /// </summary>
        public void SetArcCurve(float height)
        {
            verticalCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
            verticalCurve.AddKey(0.5f, 1f);
            curveHeight = height;
            curveAxis = CurveAxis.VERTICAL;
            _isDirty = true;
            ApplyCurve();
        }
        
        /// <summary>
        /// Set a preset wave curve
        /// </summary>
        public void SetWaveCurve(float amplitude, int frequency = 2)
        {
            verticalCurve = new AnimationCurve();
            for (int i = 0; i <= frequency * 4; i++)
            {
                float t = i / (float)(frequency * 4);
                float value = Mathf.Sin(t * Mathf.PI * 2 * frequency);
                verticalCurve.AddKey(t, value);
            }
            curveHeight = amplitude;
            curveAxis = CurveAxis.VERTICAL;
            _isDirty = true;
            ApplyCurve();
        }
        
        /// <summary>
        /// Animate curve height over time
        /// </summary>
        public void AnimateCurveHeight(float targetHeight, float duration)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateCurveHeightCoroutine(targetHeight, duration));
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private void OnTextChanged(Object obj)
        {
            if (obj == TextMeshPro && updateMode == UpdateMode.ON_TEXT_CHANGE)
            {
                _isDirty = true;
            }
        }
        
        private void ApplyCharacterRotation(Vector3[] vertices, int vertexIndex, float t)
        {
            // Calculate tangent of the curve at position t
            float delta = 0.01f;
            float y1 = verticalCurve.Evaluate(Mathf.Max(0, t - delta)) * curveHeight;
            float y2 = verticalCurve.Evaluate(Mathf.Min(1, t + delta)) * curveHeight;
            
            float angle = Mathf.Atan2(y2 - y1, delta * 2) * Mathf.Rad2Deg;
            
            // Get character center
            Vector3 center = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) * 0.5f;
            
            // Rotate each vertex around the center
            for (int i = 0; i < 4; i++)
            {
                Vector3 dir = vertices[vertexIndex + i] - center;
                float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
                
                float newX = dir.x * cos - dir.y * sin;
                float newY = dir.x * sin + dir.y * cos;
                
                vertices[vertexIndex + i] = center + new Vector3(newX, newY, dir.z);
            }
        }
        
        private System.Collections.IEnumerator AnimateCurveHeightCoroutine(float targetHeight, float duration)
        {
            float startHeight = curveHeight;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                curveHeight = Mathf.Lerp(startHeight, targetHeight, t);
                _isDirty = true;
                yield return null;
            }
            
            curveHeight = targetHeight;
            _isDirty = true;
        }
        
        #endregion
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CurvedTextMeshPro))]
    public class CurvedTextMeshProEditor : Editor
    {
        private CurvedTextMeshPro script;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (CurvedTextMeshPro)target;
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Curve", GUILayout.Height(30)))
            {
                script.ApplyCurve();
            }
            if (GUILayout.Button("Reset", GUILayout.Height(30)))
            {
                script.ResetCurve();
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Arc Curve"))
            {
                script.SetArcCurve(20f);
                EditorUtility.SetDirty(script);
            }
            if (GUILayout.Button("Wave Curve"))
            {
                script.SetWaveCurve(10f, 2);
                EditorUtility.SetDirty(script);
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}