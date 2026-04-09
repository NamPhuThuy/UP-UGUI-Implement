using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.DataManage
{
    [Serializable]
    public class PAlbumData
    {
        public bool isUnlockedAllAnimePics;
        public bool isUnlockedAllSexyPics;
        public bool isUnlockedAllUniformPics;
        public List<PAlbumRecord> playerAlbums;
        private int currentProgressAlbumId;
        public int CurrentProgressAlbumId
        {
            get
            {
                return currentProgressAlbumId;
            }
            set
            {
                currentProgressAlbumId = value;
                DataManager.Ins.MarkDirty();
            }
        }


        #region Private Serializable Fields

        [NonSerialized] private Dictionary<int, PAlbumRecord> _albumRecordById;
        [NonSerialized] private Dictionary<PictureStyle, int> _dictStyleCount;
        public Dictionary<PictureStyle, int> DictStyleCount
        {
            get
            {
                _dictStyleCount = new Dictionary<PictureStyle, int>();

                foreach (PAlbumRecord pAlbumRecord in playerAlbums)
                {
                    /*PictureStyle style = DataManager.Ins.AlbumData.data[pAlbumRecord.albumId].style;
                        
                    if (!_dictStyleCount.TryAdd(style, 1))
                    {
                        _dictStyleCount[style]++;
                    }*/
                }
                    
                return _dictStyleCount;
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        
        private void EnsureInitDict()
        {
            if (_albumRecordById != null) return;
            _albumRecordById = new Dictionary<int, PAlbumRecord>(playerAlbums != null ? playerAlbums.Count : 0);
            if (playerAlbums == null) return;

            for (int i = 0; i < playerAlbums.Count; i++)
            {
                var p = playerAlbums[i];
                if (p == null) continue;
                // Last-one-wins if duplicates exist; ideally ensure unique IDs upstream
                _albumRecordById[p.albumId] = p;
            }
        }
        
        public void RebuildDict()
        {
            _albumRecordById = null;
            EnsureInitDict();
        }

        #endregion

        public bool IsContain(int albumId)
        {
            EnsureInitDict();
            return _albumRecordById.ContainsKey(albumId);
        }

        public bool TryUnlockAlbum(int albumId)
        {
            Debug.Log(message:$"albumId: {albumId}");
            EnsureInitDict();
            if (_albumRecordById.ContainsKey(albumId)) return false;

            if (playerAlbums == null)
                playerAlbums = new List<PAlbumRecord>();
            /*AlbumRecord albumRecord = DataManager.Ins.AlbumData.data[albumId];

            var pp = new PAlbumRecord()
            {
                albumId = albumId,
                likeState = AlbumData.LikeState.NONE,
                isDownloadedVideo = false,
                isDownloadedImage = false,
                style = albumRecord.style,
                rarity = albumRecord.rarity
            };

            // Add in the dictionary
            PictureStyle style = albumRecord.style;
            if (!DictStyleCount.TryAdd(style, 1))
            {
                DictStyleCount[style]++;
            }

            playerAlbums.Add(pp);
            _albumRecordById[albumId] = pp;

            DataManager.Ins.MarkDirty();
            MMEventManager.TriggerEvent(new EPictureUnlocked(albumId));*/
            return true;
        }

        public PAlbumRecord GetRecord(int albumId)
        {
            EnsureInitDict();
            return _albumRecordById.GetValueOrDefault(albumId);
        }

        public bool IsUnlockAll()
        {
            // return playerAlbums.Count == DataManager.Ins.AlbumData.data.Count;
            return true;
        }
    }

    [Serializable]
    public class PAlbumRecord
    {
        public bool isDownloadedVideo;
        public bool isDownloadedImage;
        public int albumId;
        public AlbumData.LikeState likeState;
        public PictureRarity rarity;
        public PictureStyle style;
    }
}
