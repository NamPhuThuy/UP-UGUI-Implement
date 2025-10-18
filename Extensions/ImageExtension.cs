/*
Github: https://github.com/NamPhuThuy
*/

using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UI
{

    public static class ImageExtension
    {
        /// <summary>
        /// Fits the Image to the given RectTransform while maintaining aspect ratio.
        /// If 'cover' is true, the image will fill the rect (like background-size: cover).
        /// If false, the image will fit fully inside the rect (like background-size: contain).
        /// </summary>
        public static void FitImageToRectTransform(this Image image, RectTransform targetRect = null, bool cover = false)
        {
            RectTransform rt = image.rectTransform;

            float imageWidth = image.sprite.rect.width;
            float imageHeight = image.sprite.rect.height;
            float imageAspect = imageWidth / imageHeight;

            float screenWidth;
            float screenHeight;
            
            if (targetRect != null)
            {
                screenWidth = targetRect.sizeDelta.x;
                screenHeight = targetRect.sizeDelta.y;
            }
            else
            {
                screenWidth = UIConst.CANVAS_SIZE.x;
                screenHeight = UIConst.CANVAS_SIZE.y;
            }

            float screenAspect = screenWidth / screenHeight;

            float targetWidth;
            float targetHeight;

            if (imageAspect < screenAspect)
            {
                targetWidth = screenWidth;
                targetHeight = targetWidth / imageAspect;
            }
            else
            {
                targetHeight = screenHeight;
                targetWidth = targetHeight * imageAspect;
            }

            rt.sizeDelta = new Vector2(targetWidth, targetHeight);
            rt.localPosition = Vector2.zero;
        }
    }
}