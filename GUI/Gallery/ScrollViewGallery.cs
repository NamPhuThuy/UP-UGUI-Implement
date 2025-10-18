using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NamPhuThuy.UI
{
    public class ScrollViewGallery : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("View")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private ElementGallery elementPrefab;
        [SerializeField] private GridLayoutGroup _contentGridLayoutGroup;
        [SerializeField] private ScrollRect scrollRect;
        public ScrollRect ScrollRect => scrollRect;

        private Vector2 _elementSpacing;
        private Vector2 _elementSize;
        private Vector2 _contentBuffer = new Vector2(0f, 250f);

        [Header("Components")]
        [SerializeField] private List<ElementGallery> elementList;
        #endregion

        #region Public Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            //GET REFERENCES
            _contentGridLayoutGroup = _content.GetComponent<GridLayoutGroup>();
            scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            
        }

        #endregion

        #region Private Methods

        #endregion

        #region Update Methods

        private void UpdateSizesInfo()
        {
            _elementSize = _contentGridLayoutGroup.cellSize;
            // _elementSpacing = _contentGridLayoutGroup.spacing;
            _elementSpacing = new Vector2(0f, _contentGridLayoutGroup.spacing.y);

            _elementSize = new Vector2(_contentGridLayoutGroup.cellSize.x * (scrollRect.horizontal ? 1 : 0), _contentGridLayoutGroup.cellSize.y * (scrollRect.vertical ? 1 : 0));
        }
        
        public void UpdateContentSizeDelta(int itemNum)
        {
            int rowNum = Mathf.CeilToInt((float)itemNum / _contentGridLayoutGroup.constraintCount);
            
            _content.sizeDelta = rowNum * _elementSize +
                                 (rowNum + 1) * _elementSpacing +
                                 _contentBuffer;
        }
       
        
        #endregion

        #region Public Methods

        public void InitContent()
        {
            UpdateSizesInfo(); //GET THE SIZES
            UpdateContentSizeDelta(UIConst.SCROLL_VIEW_TEST_ITEM_NUM);

            for (int i = 0; i < UIConst.SCROLL_VIEW_TEST_ITEM_NUM; i++)
            {
                AddElement();   
            }
        }

        
        
        #endregion

        #region Control Elements
        
        private void AddElement()
        {
            ElementGallery element = Instantiate(elementPrefab, _content.transform);
            elementList.Add(element);

            //Assign value 
            SetupElementInfo();

            element.gameObject.SetActive(true);

            void SetupElementInfo()
            {
                
            }
        }
        #endregion
    }
}