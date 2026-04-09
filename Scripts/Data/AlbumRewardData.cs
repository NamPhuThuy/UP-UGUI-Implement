using System.Collections.Generic;
using NamPhuThuy.Common;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.DataManage
{
    [CreateAssetMenu(fileName = "AlbumRewardData", menuName = "Game/AlbumRewardData", order = 1)]
    public class AlbumRewardData : ScriptableObject
    {
        [SerializeField] private AlbumRewardRecord[] data;

        private Dictionary<int, AlbumRewardRecord> _dictData;

        public Dictionary<int, AlbumRewardRecord> Data
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

        #region MonoBehaviour Callbacks

        private void OnValidate()
        {
#if UNITY_EDITOR
            bool changed = false;
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
                _dictData = null; // force rebuild after edits   
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
                _dictData = new Dictionary<int, AlbumRewardRecord>(0);
                return;
            }

            _dictData = new Dictionary<int, AlbumRewardRecord>(data?.Length ?? 0);
            if (_dictData == null) return;
            foreach (var r in data)
            {
                if (r == null) continue;
                _dictData[r.recordId] = r;
            }
        }

        #endregion

        #region Public Methods

        public AlbumRewardRecord GetRecord(int recordId)
        {
            EnsureInitDict();
            return _dictData.GetValueOrDefault(recordId);
        }

        public void ApplyRemoteData(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            _dictData = null;
        }

        public int GetCurrentMilestone(int currentLevel)
        {
            DebugLogger.Log(message: $"currentLevel: {currentLevel}");
            if (data == null || data.Length == 0) return -1;

            int currentTotalLevel = 0;
            for (int i = 0; i < data.Length; i++)
            {
                currentTotalLevel += data[i].milestone;
                if (currentLevel <= currentTotalLevel)
                {
                    return data[i].milestone;
                }
            }

            return -1;
        }

        public float GetCurrentProgress(int currentLevel)
        {
            if (data == null || data.Length == 0)
            {
                int remainder = ((currentLevel - 1) % DataConst.DEFAULT_GALLERY_MILESTONE) + 1;
                return remainder / (float)DataConst.DEFAULT_GALLERY_MILESTONE;
            }

            int currentLevelAmount = currentLevel;

            for (int i = 0; i < data.Length; i++)
            {
                if (currentLevelAmount > data[i].milestone)
                {
                    currentLevelAmount -= data[i].milestone;
                }
                else
                {
                    return currentLevelAmount/(float)data[i].milestone;
                }
            }

            // Fallback: currentLevel exceeded all records in data
            int rem = ((currentLevelAmount - 1) % DataConst.DEFAULT_GALLERY_MILESTONE) + 1;
            return rem / (float)DataConst.DEFAULT_GALLERY_MILESTONE;
        }

        public float GetPrevProgress(int currentLevel)
        {
            float progress = GetCurrentProgress(currentLevel - 1);

            return Mathf.Approximately(progress, 1) ? 0 : progress;
        }

        #endregion
    }

    [System.Serializable]
    public class AlbumRewardRecord
    {
        [Tooltip("Auto-assigned ID, corresponds to its index in the list.")]
        public int recordId;

        public int milestone;
        public List<ResourceAmount> rewards;
    }
}