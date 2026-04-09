using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.DataManage;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.GirlGallery
{
    [CreateAssetMenu(fileName = "GalleryPack", menuName = "Game/GalleryPack", order = 1)]
    public class GalleryPackData : ScriptableObject
    {
        [SerializeField] public GalleryPackRecord[] data;

        private Dictionary<int, GalleryPackRecord> _dictData;

        public Dictionary<int, GalleryPackRecord> Data
        {
            get
            {
                if (_dictData == null)
                {
                    EnsureInitDict();
                }

                return _dictData;
            }
        }

        #region Editor Lifecycle

        private void OnValidate()
        {
#if UNITY_EDITOR
            // Auto-assign recordId by its index in the array.
            bool changed = false;
            if (data == null) return;

            for (int i = 0; i < data.Length; i++)
            {
                var r = data[i];
                if (r == null) continue;
                if (r.recordId != i)
                {
                    r.recordId = i;
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(this);
                _dictData = null; // Force rebuild after edits.
            }
#endif
        }

        #endregion

        #region Private Methods

        private void EnsureInitDict()
        {
            if (_dictData != null) return;
            InitDict();
        }

        private void InitDict()
        {
            if (data == null || data.Length == 0)
            {
                _dictData = new Dictionary<int, GalleryPackRecord>(0);
                return;
            }

            _dictData = new Dictionary<int, GalleryPackRecord>(data.Length);
            foreach (var r in data)
            {
                if (r == null) continue;
                _dictData[r.recordId] = r; // Last one wins if there are duplicate IDs.
            }
        }

        #endregion

        #region Public Methods

        public GalleryPackRecord GetRecord(int recordId)
        {
            EnsureInitDict();
            return _dictData.GetValueOrDefault(recordId);
        }

        #endregion
    }

    [System.Serializable]
    public class GalleryPackRecord
    {
        // Note: The [HelpBox] attribute is a custom attribute.
        // Please ensure it is defined in your project.
        [HelpBox("auto-assigned id", HelpBoxMessageType.Info)]
        public int recordId;

        public string packName;
        public PictureStyle packType;
        public Sprite backgroundSprite;
        public Sprite titleSprite;
        public Sprite descriptionSprite;
        public int price;
    }
}