using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    public class DeviceScreenSizeDetector : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(DelayDetect());
        }

        private IEnumerator DelayDetect()
        {
            yield return new WaitForEndOfFrame();

            RectTransform canvas = GetComponent<RectTransform>();

            // GamePersistentVariable.canvasSize = canvas.sizeDelta;
        }
    }

}

