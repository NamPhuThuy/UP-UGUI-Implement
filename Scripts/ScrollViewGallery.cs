using System;
using System.Collections.Generic;
using System.Linq;
using NamPhuThuy.Common;
using NamPhuThuy.DataManage;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{
    [System.Serializable]
    public struct ColumnBreakpoint
    {
        [Tooltip("Minimum content width (canvas units) to use this column count")]
        public float minContentWidth;
        public int columns;
        [Tooltip("Spacing as a fraction of cell width for this breakpoint (e.g. 0.04 = 4%)")]
        public float spacingRatio;
    }

    public class ScrollViewGallery : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("View")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private ElementGallery elementPrefab;
        [SerializeField] private GridLayoutGroup contentGridLayoutGroup;
        [SerializeField] private ScrollRect scrollRect;
        public ScrollRect ScrollRect => scrollRect;

        [Header("Adaptive Layout")]
        [Tooltip("Spacing as a fraction of cell width (e.g. 0.06 = 6%). Applied at runtime to adapt phone vs iPad.")]
        private float spacingRatio = 0.06f;

        [Tooltip("Sorted ascending by minContentWidth. The highest entry whose minContentWidth <= actual width wins.")]
        [SerializeField] private ColumnBreakpoint[] columnBreakpoints = new ColumnBreakpoint[]
        {
            new ColumnBreakpoint { minContentWidth = 0f,    columns = 3, spacingRatio = 0.03f },  // phone portrait
            new ColumnBreakpoint { minContentWidth = 1100f, columns = 4, spacingRatio = 0.04f },  // large phone / small tablet
            new ColumnBreakpoint { minContentWidth = 1200f, columns = 5, spacingRatio = 0.05f },  // iPad
        };

        private Vector2 _elementSpacing;
        private Vector2 _elementSize;
        private Vector2 _contentBuffer = new Vector2(0f, 250f);
        private float _originalCellAspect = -1f;

        [Header("Components")]
        [SerializeField] private List<ElementGallery> elementList;
        public List<ElementGallery> ElementList => elementList;
        private HashSet<int> elementPictureId = new HashSet<int>();
        private Dictionary<int, int> _dictPictureToElementId = new Dictionary<int, int>(); // Map the element-id to the correspond pictureId
        #endregion

        #region Public Fields

        public Action onEnable;

        #endregion

        #region Private Fields

        private GUIGallery.FilterChoice _currentFilterChoice;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            contentGridLayoutGroup = _content.GetComponent<GridLayoutGroup>();
            scrollRect = GetComponent<ScrollRect>();

            // Capture the inspector-set cell aspect ratio once, before any runtime scaling
            if (_originalCellAspect < 0 && contentGridLayoutGroup.cellSize.x > 0)
                _originalCellAspect = contentGridLayoutGroup.cellSize.y / contentGridLayoutGroup.cellSize.x;
        }

        private void OnEnable()
        {
            AdaptLayoutToScreen(); //ADAPT SPACING & CELL SIZE TO CURRENT SCREEN
            UpdateSizesInfo(); //GET THE SIZES
            // UpdateContentSizeDelta(DataManager.Ins.PAlbumData.playerAlbums.Count);

            scrollRect.LockVerticalScrollALittle();

            onEnable?.Invoke();

            UpdateElements();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recomputes cellSize and spacing so the grid fills the content panel evenly
        /// on any screen (phone portrait, iPad, etc.).
        /// Formula: cols * cellW + (cols-1) * spacingX = availableWidth
        ///          spacingX = cellW * spacingRatio
        /// </summary>
        private void AdaptLayoutToScreen()
        {
            Canvas.ForceUpdateCanvases();

            float contentWidth = _content.rect.width;
            if (contentWidth <= 0) return;

            // Capture aspect ratio on first run if Start() hasn't fired yet
            if (_originalCellAspect < 0 && contentGridLayoutGroup.cellSize.x > 0)
                _originalCellAspect = contentGridLayoutGroup.cellSize.y / contentGridLayoutGroup.cellSize.x;

            float aspect = _originalCellAspect > 0 ? _originalCellAspect : 1f;

            RectOffset pad = contentGridLayoutGroup.padding;
            
            
            float available = contentWidth - pad.left - pad.right;
            
            DebugLogger.Log(message:$"available: {available} = contentWidth: {contentWidth} - pad.left: {pad.left} - pad.right: {pad.right}");

            // Pick column count and spacingRatio from breakpoints (highest minContentWidth that fits)
            int cols = contentGridLayoutGroup.constraintCount; // fallback
            float activeSpacingRatio = spacingRatio;
            if (columnBreakpoints != null && columnBreakpoints.Length > 0)
            {
                foreach (ColumnBreakpoint bp in columnBreakpoints)
                {
                    if (available >= bp.minContentWidth)
                    {
                        cols = bp.columns;
                        activeSpacingRatio = bp.spacingRatio;
                    }
                }
            }
            contentGridLayoutGroup.constraintCount = cols;

            // Solve for cellW: cols*cellW + (cols-1)*cellW*activeSpacingRatio = available
            float cellW = available / (cols + (cols - 1) * activeSpacingRatio);
            float spacingX = cellW * activeSpacingRatio;

            // contentGridLayoutGroup.cellSize = new Vector2(cellW, cellW * aspect);
            contentGridLayoutGroup.spacing = new Vector2(spacingX, spacingX);
        }

        private void HideAllElements()
        {
            foreach (ElementGallery element in elementList)
            {
                element.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Update Methods

        private void UpdateSizesInfo()
        {
            _elementSize = contentGridLayoutGroup.cellSize;
            // _elementSpacing = _contentGridLayoutGroup.spacing;
            _elementSpacing = new Vector2(0f, contentGridLayoutGroup.spacing.y);

            _elementSize = new Vector2(contentGridLayoutGroup.cellSize.x * (scrollRect.horizontal ? 1 : 0), contentGridLayoutGroup.cellSize.y * (scrollRect.vertical ? 1 : 0));
        }

        public void UpdateContentSizeDelta(int itemNum)
        {
            int rowNum = Mathf.CeilToInt((float)itemNum / contentGridLayoutGroup.constraintCount);
            _content.sizeDelta = rowNum * _elementSize +
                                 (rowNum + 1) * _elementSpacing +
                                 _contentBuffer;
        }

        public List<int> UpdateContent(GUIGallery.FilterChoice filterChoice)
        {
            HideAllElements();

            Dictionary<int, int> dictPictureToElementId = new Dictionary<int, int>(); // Map the element-id to the correspond pictureId
            List<int> finalPictureIdList = new List<int>();

            for (int i = 0; i < elementList.Count; i++)
            {
                dictPictureToElementId.Add(elementList[i].pictureId, i); // pictureId - elementId
                finalPictureIdList.Add(elementList[i].pictureId);
            }

            // Filter with Rarity first
            List<int> tempList = new List<int>();
            foreach (int id in finalPictureIdList)
            {
                /*if (DataManager.Ins.AlbumData.data[id].rarity == filterChoice.Rarity)
                    tempList.Add(id);*/
            }

            finalPictureIdList.Clear();
            finalPictureIdList = tempList.DeepCopy();
            tempList.Clear();

            DebugResultList(filterChoice.Rarity.ToString());

            switch (filterChoice.Kind)
            {
                case GUIGallery.FilterKind.All:
                    // No extra filter
                    DebugResultList("Nothing");
                    break;

                case GUIGallery.FilterKind.LikeState:
                    foreach (int id in finalPictureIdList)
                    {
                        /*if (DataManager.Ins.PAlbumData.playerAlbums[dictPictureToElementId[id]].likeState == filterChoice.LikeState)
                            tempList.Add(id);*/
                    }

                    finalPictureIdList.Clear();
                    finalPictureIdList = tempList.DeepCopy();
                    tempList.Clear();

                    DebugResultList(filterChoice.LikeState.ToString());
                    break;
                case GUIGallery.FilterKind.Style:
                    foreach (int id in finalPictureIdList)
                    {
                        /*if (DataManager.Ins.AlbumData.data[id].style == filterChoice.Style)
                            tempList.Add(id);*/
                    }
                    finalPictureIdList.Clear();
                    finalPictureIdList = tempList.DeepCopy();
                    tempList.Clear();

                    DebugResultList(filterChoice.Style.ToString());
                    break;
                default:
                    break;
            }

            DebugLogger.Log(message:$"ScrollViewGallery.UpdateContent() total element {finalPictureIdList.Count}");
            DebugResultList("everything");

            if (finalPictureIdList.Count <= 0)
            {
                return new List<int>();
            }

            UpdateContentSizeDelta(finalPictureIdList.Count);
            foreach (int i in finalPictureIdList)
            {
                elementList[dictPictureToElementId[i]].gameObject.SetActive(true);
            }

            _currentFilterChoice = filterChoice;

            void DebugResultList(string filterType)
            {
                string indexString = "";
                foreach (int i in finalPictureIdList)
                    indexString += $"{i.ToString()} ";

                DebugLogger.Log(message:$"ScrollViewGallery: after {filterType} filter indexes: {indexString}");
            }

            return finalPictureIdList;
        }

        #endregion

        #region Public Methods

        public void InitContent()
        {
            /*
            foreach (PAlbumRecord p in DataManager.Ins.PAlbumData.playerAlbums)
            {
                AddElement(p);
            }
            */

            UpdateElements();
        }

        #endregion

        #region Control Elements

        private void AddElement(PAlbumRecord p)
        {
            if (!elementPictureId.Add(p.albumId)) return;
         
            /*ElementGallery element = Instantiate(elementPrefab, _content.transform);
            elementList.Add(element);

            AlbumRecord pictureData = DataManager.Ins.AlbumData.data[p.albumId];

            element.pictureId = p.albumId;

            if (pictureData.IsLocalAvailable())
            {
                //Assign value 
                element.contentImage.sprite = pictureData.localMainSprite;
                element.likeState = p.likeState;
                element.FitImageToRectTransform();

                element.HidePlaceholderFast();
            }
            else
            {
                element.ShowPlaceholder();
                // _ = DataManager.Ins.LoadRemotePictureData(element.pictureId);
            }

            element.ToggleAnimatedTag(false);
            element.gameObject.SetActive(true);*/
        }

        public void UpdateElements()
        {
            /*List<int> pictureIds = new List<int>();

            List<AlbumRecord> pictureDatas = DataManager.Ins.AlbumData.data;

            List<ElementGallery> sortedElements = elementList;

            try
            {
                sortedElements = elementList
                    .OrderBy(x => pictureDatas[x.pictureId].rarity != _currentFilterChoice.Rarity)
                    .ToList();
            }
            catch (System.Exception)
            {
                
            }

            // remote element
            foreach (var element in sortedElements)
            {
                if (DataManager.Ins.AlbumData.data.Any(pictureData => pictureData.recordId == element.pictureId))
                {
                    AlbumRecord albumRecord = DataManager.Ins.AlbumData.data.First(pictureData => pictureData.recordId == element.pictureId);

                    if (albumRecord.IsLocalAvailable())
                    {
                        //Assign value 
                        element.contentImage.sprite = albumRecord.localMainSprite;
                        element.FitImageToRectTransform();

                        element.HidePlaceholder();
                    }
                    else
                    {
                        element.ShowPlaceholder();
                    }

                    pictureIds.Add(albumRecord.recordId);
                }
            }

            StartCoroutine(DataManager.Ins.FetchPictureDatas(pictureIds));*/
        }

        #endregion
    }
}