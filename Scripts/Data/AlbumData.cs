using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace NamPhuThuy.DataManage
{
    [CreateAssetMenu(fileName = "AlbumData", menuName = "Game/AlbumData", order = 1)]
    public class AlbumData : ScriptableObject
    {
        public List<AlbumRecord> data = new List<AlbumRecord>();

        private Dictionary<int, AlbumRecord> _dictData;

        public Dictionary<int, AlbumRecord> Data
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


        public List<ResourceAmount> downloadVideoPrice;
        public List<ResourceAmount> downloadImagePrice;

        public int animeAlbumNums = 0;
        public int sexyAlbumNums = 0;
        public int uniformAlbumNums = 0;

        #region MonoBehaviour Callbacks

        private void OnValidate()
        {
#if UNITY_EDITOR
            bool changed = false;
            for (int i = 0; i < data.Count; i++)
            {
                var r = data[i];
                if (r == null) continue;
                if (r.recordId != i)
                {
                    r.recordId = i;
                    changed = true;
                }

                if (i < 36)
                {
                    r.videoUrl = $"Videos/Video {i}";
                    changed = true;
                }
                else
                {
                    r.videoUrl = "";
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
            if (data == null || data.Count == 0)
            {
                _dictData = new Dictionary<int, AlbumRecord>(0);
                return;
            }

            _dictData = new Dictionary<int, AlbumRecord>(data?.Count ?? 0);
            if (_dictData == null) return;
            foreach (var r in data)
            {
                if (r == null) continue;
                _dictData[r.recordId] = r;
            }
        }
        
       

        #endregion

        #region Public Methods

        public AlbumRecord GetRecord(int recordId)
        {
            EnsureInitDict();
            return _dictData.GetValueOrDefault(recordId);
        }

        public int AlbumAmountOnStyle(PictureStyle style) => style switch
        {
            PictureStyle.ANIME => animeAlbumNums,
            PictureStyle.SEXY => sexyAlbumNums,
            PictureStyle.UNIFORM => uniformAlbumNums,
            _ => 0
        };

        // Call this to get the right sprite for any record
        // Shows local immediately, loads remote if available
        public static async Task<Sprite> LoadMainImage(
            AlbumRecord record,
            System.Action<Sprite> onSpriteReady) // callback for UI update
        {
            // Step 1 — Show local immediately if available
            if (record.IsLocalAvailable())
            {
                onSpriteReady?.Invoke(record.GetLocalMain());

                // If no remote configured, we're done
                if (!record.HasRemoteContent)
                    return record.GetLocalMain();
            }

            // Step 2 — Load remote (replaces local in UI when ready)
            if (!record.HasRemoteContent)
            {
                Debug.LogWarning($"[AlbumLoader] Record {record.recordId} has no local or remote image.");
                return null;
            }

            var handle = record.remoteMainImage.LoadAssetAsync<Sprite>();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onSpriteReady?.Invoke(handle.Result); // swap to higher quality remote
                return handle.Result;
            }

            Debug.LogWarning($"[AlbumLoader] Failed to load remote image for record {record.recordId}");
            return record.GetLocalMain(); // fallback to local
        }

        public static void ReleaseRemoteImage(AlbumRecord record)
        {
            if (record.remoteMainImage != null && record.remoteMainImage.IsValid())
                record.remoteMainImage.ReleaseAsset();
        }
        
        

        #endregion

        public enum LikeState
        {
            NONE = 0,
            LIKE = 1,
            DISLIKE = 2,
        }
    }

    [System.Serializable]
    public class AlbumRecord
    {
        [Tooltip("Auto-assigned ID, corresponds to its index in the list.")]
        public int recordId;

        [Header("Details")] public string albumName;
        [TextArea] public string description;
        [FormerlySerializedAs("albumCover")] public Sprite localAlbumCover;
        [FormerlySerializedAs("mainImage")] public Sprite localMainSprite;

        [Header("Remote Sprites (Addressables — no hard ref)")]
        public AssetReferenceSprite remoteAlbumCover; // GUID only, not baked
        public AssetReferenceSprite remoteMainImage; // GUID only, not baked

        public PictureRarity rarity;
        public PictureStyle style;
        public string videoUrl; // using streamingAssets
        public bool isDownloadingFromRemote = false;
        
        [NonSerialized] public Sprite cachedRemoteSprite;       // loaded remote sprite
        [NonSerialized] public DownloadState downloadState = DownloadState.None;

        public bool IsLocalAvailable()
        {
            return localMainSprite != null;
        }

        /// True if remote has been downloaded and cached by Addressables
        public bool IsRemoteAvailable => remoteMainImage != null
                                         && remoteMainImage.IsValid();

        /// True if remote address is configured (doesn't mean downloaded yet)
        public bool HasRemoteContent => remoteMainImage != null
                                        && !string.IsNullOrEmpty(remoteMainImage.AssetGUID);


        /// Returns local sprite immediately — call this for UI that needs instant display
        public Sprite GetLocalCover() => localAlbumCover;

        public Sprite GetLocalMain() => localMainSprite;
    }

    public enum PictureRarity
    {
        NONE = 0,
        NORMAL = 1,
        SPECIAL = 2
    }

    public enum PictureStyle
    {
        NONE = 0,
        ANIME = 1,
        SEXY = 2,
        UNIFORM = 3
    }
    
    public enum DownloadState
    {
        None,           // not started
        Downloading,    // in progress
        Done,           // successfully cached
        Failed          // error
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AlbumData))]
    public class AlbumDataEditor : Editor
    {
        private DefaultAsset targetFolder;
        private AlbumData _script;

        private void OnEnable()
        {
            _script = (AlbumData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Batch Import", EditorStyles.boldLabel);
            targetFolder =
                (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", targetFolder, typeof(DefaultAsset), false);

            if (GUILayout.Button("Load Images from Folder"))
            {
                LoadFromFolder();
            }

            if (GUILayout.Button("Set Style-nums"))
            {
                SetStyleNums();
            }
        }

        /*private void LoadFromFolder()
        {
            if (targetFolder == null)
            {
                Debug.LogError("Please select a target folder.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(targetFolder);
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });

            AlbumData albumData = (AlbumData)target;
            Undo.RecordObject(albumData, "Batch Import Album Records");

            var rarities = ((PictureRarity[])Enum.GetValues(typeof(PictureRarity))).Where(r => r != PictureRarity.NONE).ToArray();
            var styles = ((PictureStyle[])Enum.GetValues(typeof(PictureStyle))).Where(s => s != PictureStyle.NONE).ToArray();

            int count = 0;
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite != null)
                {
                    // Check if already exists by sprite reference to avoid duplicates
                    if (albumData.data.Any(r => r.localMainSprite == sprite))
                    {
                        continue;
                    }

                    AlbumRecord record = new AlbumRecord();
                    record.albumName = sprite.name;
                    record.localAlbumCover = sprite;
                    record.localMainSprite = sprite;

                    // Randomize
                    record.rarity = rarities[UnityEngine.Random.Range(0, rarities.Length)];
                    record.style = styles[UnityEngine.Random.Range(0, styles.Length)];

                    albumData.data.Add(record);
                    count++;
                }
            }

            EditorUtility.SetDirty(albumData);
            AssetDatabase.SaveAssets();
            Debug.Log($"Imported {count} new images into AlbumData.");
        }*/

        #region Private Methods

        private AssetReferenceSprite CreateSpriteReference(string guid, Sprite sprite)
        {
#if UNITY_EDITOR
            // Step 1 — Ensure the texture is registered in Addressables
            EnsureAddressableEntry(guid, sprite.name);

            // Step 2 — Build the reference with correct sub-object
            var reference = new AssetReferenceSprite(guid);

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (allAssets.Length > 1)
                reference.SetEditorSubObject(sprite);

            return reference;
#else
    return new AssetReferenceSprite(guid);
#endif
        }

#if UNITY_EDITOR
        private void EnsureAddressableEntry(string guid, string address)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[AlbumData] Addressable Settings not found!");
                return;
            }

            // Check if already registered
            var existingEntry = settings.FindAssetEntry(guid);
            if (existingEntry != null) return; // already an addressable, skip

            // Find or create the target group — put remote sprites in their own group
            const string groupName = "AlbumSprites_Remote";
            var group = settings.FindGroup(groupName)
                        ?? settings.CreateGroup(groupName, false, false, true, null);

            // Register the texture as an Addressable with the sprite name as address
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = address; // e.g. "character_01"

            settings.SetDirty(
                AddressableAssetSettings.ModificationEvent.EntryMoved,
                entry, true);

            Debug.Log($"[AlbumData] Registered '{address}' into Addressables group '{groupName}'");
        }
#endif

        #endregion
        
        private void LoadFromFolder()
        {
            if (targetFolder == null)
            {
                Debug.LogError("Please select a target folder.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(targetFolder);
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });

            AlbumData albumData = (AlbumData)target;

            // ── Step 1: Determine local sprite amount ─────────────────────
            // Pop-up dialog asking how many sprites should ship locally
            // The rest will be assigned to AssetReference (remote) fields only
            int totalFound = guids.Length;
            int localCount = 0;

            bool confirmed = EditorUtility.DisplayDialog(
                "Configure Local Sprites",
                $"Found {totalFound} sprites in folder.\n\n" +
                "How many should ship locally (built into APK)?\n" +
                "The rest will be remote-only (AssetReference, no hard ref).\n\n" +
                "You'll enter the count in the next dialog.",
                "Continue", "Cancel"
            );

            if (!confirmed) return;

            // Use a small Editor Window to get numeric input
            LocalCountPickerWindow.Show(totalFound, pickedCount =>
            {
                localCount = pickedCount;
                ExecuteImport(albumData, guids, localCount);
            });
        }

        private void ExecuteImport(AlbumData albumData, string[] guids, int localCount)
        {
            Undo.RecordObject(albumData, "Batch Import Album Records");

            var rarities = ((PictureRarity[])Enum.GetValues(typeof(PictureRarity)))
                .Where(r => r != PictureRarity.NONE).ToArray();
            var styles = ((PictureStyle[])Enum.GetValues(typeof(PictureStyle)))
                .Where(s => s != PictureStyle.NONE).ToArray();

            int imported = 0;
            int localAssigned = 0;
            int remoteOnly = 0;

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null) continue;

                // Skip duplicates — check both local and remote fields
                bool alreadyExists = albumData.data.Any(r =>
                    r.localMainSprite == sprite ||
                    (r.remoteMainImage != null && r.remoteMainImage.AssetGUID == guids[i]) ||
                    (r.remoteAlbumCover != null && r.remoteAlbumCover.AssetGUID == guids[i])
                );
                if (alreadyExists) continue;
                
                
                // ✅ Correctly built reference with sub-object + auto-registered
                AssetReferenceSprite spriteRef = CreateSpriteReference(guids[i], sprite);

                // ── Step 2: Assign to correct fields based on localCount ──
                bool isLocal = localAssigned < localCount;
                
                var record = new AlbumRecord
                {
                    albumName       = sprite.name,
                    rarity          = rarities[UnityEngine.Random.Range(0, rarities.Length)],
                    style           = styles[UnityEngine.Random.Range(0, styles.Length)],
                    localAlbumCover = isLocal ? sprite : null,  // hard ref only if local
                    localMainSprite = isLocal ? sprite : null,  // hard ref only if local
                    remoteAlbumCover = spriteRef,               // ✅ always set with sub-object
                    remoteMainImage  = spriteRef,               // ✅ always set with sub-object
                };

                if (isLocal) localAssigned++;
                else remoteOnly++;

                albumData.data.Add(record);
                imported++;
            }

            EditorUtility.SetDirty(albumData);
            AssetDatabase.SaveAssets();

            // ── Step 3: Summary report ────────────────────────────────────
            Debug.Log(
                $"[AlbumData] Import complete!\n" +
                $"  Total imported : {imported}\n" +
                $"  Local (baked)  : {localAssigned}  → localMainSprite + AssetReference both set\n" +
                $"  Remote only    : {remoteOnly}  → AssetReference only, no hard ref\n" +
                $"  Skipped (dupe) : {guids.Length - imported}"
            );

            EditorUtility.DisplayDialog(
                "Import Complete",
                $"Imported {imported} records.\n\n" +
                $"🔵 Local (baked into APK): {localAssigned}\n" +
                $"☁️  Remote only (AssetRef): {remoteOnly}\n" +
                $"⏭  Skipped duplicates: {guids.Length - imported}",
                "OK"
            );

            SetStyleNums();
        }

        private void SetStyleNums()
        {
            _script.animeAlbumNums = 0;
            _script.sexyAlbumNums = 0;
            _script.uniformAlbumNums = 0;

            foreach (AlbumRecord record in _script.data)
            {
                if (record.style == PictureStyle.ANIME)
                {
                    _script.animeAlbumNums++;
                }
                else if (record.style == PictureStyle.SEXY)
                {
                    _script.sexyAlbumNums++;
                }
                else if (record.style == PictureStyle.UNIFORM)
                {
                    _script.uniformAlbumNums++;
                }
            }

            EditorUtility.SetDirty(this);
        }
    }
#endif
}