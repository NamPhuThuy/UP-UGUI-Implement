/*
Github: https://github.com/NamPhuThuy
Supports: ScreenSpace-Overlay, ScreenSpace-Camera, WorldSpace
*/

using NamPhuThuy.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NamPhuThuy.UGUIImplement
{
    public static class ImageExtension
    {
        // Need to pass the rectTransform of the image's parent gameObject
        public static void FitImageToRectTransformScreenSpaceOverlay(
            this Image image, 
            RectTransform targetRect = null, 
            ImageFitMode fitMode = ImageFitMode.CONTAIN)
        {
            // return;
            DebugLogger.Log();
            if (image.sprite == null)
            {
                return;
            }

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
                screenWidth = UGUIConst.CANVAS_SIZE_MOBILE.x;
                screenHeight = UGUIConst.CANVAS_SIZE_MOBILE.y;
            }

            float screenAspect = screenWidth / screenHeight;

            float targetWidth;
            float targetHeight;

            Vector2 offset = Vector2.zero;

            if (imageAspect < screenAspect)
            {
                targetWidth = screenWidth;
                targetHeight = targetWidth / imageAspect;

                offset.y = -(targetHeight - screenHeight) / 2;
            }
            else
            {
                targetHeight = screenHeight;
                targetWidth = targetHeight * imageAspect;
            }

            rt.sizeDelta = new Vector2(targetWidth, targetHeight);
            rt.localPosition = Vector2.zero + offset;
        }
        
        /// <summary>
        /// Fits the Image to the given RectTransform while maintaining aspect ratio.
        /// Fully supports ScreenSpace-Camera render mode.
        /// </summary>
        /// <param name="image">The Image component to fit</param>
        /// <param name="targetRect">Target RectTransform to fit into. If null, uses canvas size.</param>
        /// <param name="fitMode">How to fit: Contain (fit inside) or Cover (fill completely)</param>
        public static void FitImageToRectTransformScreenSpaceCamera(
            this Image image, 
            RectTransform targetRect = null, 
            ImageFitMode fitMode = ImageFitMode.CONTAIN)
        {
            if (image == null || image.sprite == null)
            {
                Debug.LogWarning("[ImageExtension] Image or sprite is null!");
                return;
            }

            RectTransform imageRect = image.rectTransform;

            // Get image dimensions from sprite
            float imageWidth = image.sprite.rect.width;
            float imageHeight = image.sprite.rect.height;
            float imageRectWidth = image.rectTransform.sizeDelta.x;
            float imageRectHeight = image.rectTransform.sizeDelta.y;

            // Get target dimensions (properly handles ScreenSpace-Camera)
            Vector2 targetSize = GetTargetSize(image, targetRect);
            float targetWidth = targetSize.x;
            float targetHeight = targetSize.y;

            // Calculate scale ratios
            float widthRatio = targetWidth / imageWidth;
            float heightRatio = targetHeight / imageHeight;
            
            Debug.Log(message:$"witdhRatio: {widthRatio}, heightRatio: {heightRatio}");

            float finalWidth;
            float finalHeight;
            float selectedRatio;

            // Choose scale based on fit mode
            if (fitMode == ImageFitMode.CONTAIN)
            {
                // Fit inside: Use smaller ratio (ensures entire image fits)
                selectedRatio = Mathf.Min(widthRatio, heightRatio);
                
                finalWidth = imageWidth * selectedRatio;
                finalHeight = imageHeight * selectedRatio;
                
                /*finalWidth = imageRectWidth * selectedRatio;
                finalHeight = imageRectHeight * selectedRatio;*/
            }
            else if (fitMode == ImageFitMode.COVER)
            {
                // Fill completely: Use larger ratio (ensures no empty space)
                selectedRatio = Mathf.Max(widthRatio, heightRatio);
                
                finalWidth = imageWidth * selectedRatio;
                finalHeight = imageHeight * selectedRatio;
                
                /*finalWidth = imageRectWidth * selectedRatio;
                finalHeight = imageRectHeight * selectedRatio;*/
            }
            else // ImageFitMode.Stretch
            {
                // Stretch to fill (ignore aspect ratio)
                finalWidth = targetWidth;
                finalHeight = targetHeight;
            }

            Debug.Log(message:$"Before imageRect.sizeDelta: {imageRect.sizeDelta}");
            
            // Apply size
            imageRect.sizeDelta = new Vector2(finalWidth, finalHeight);
            
            Debug.Log(message:$"After imageRect.sizeDelta: {imageRect.sizeDelta}");
            Debug.Log(message:$"finalWidth: {finalWidth}, finalHeight: {finalHeight}");
            
            // Center the image
            imageRect.anchoredPosition = Vector2.zero;
            
            image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// Get target size - handles ScreenSpace-Camera correctly
        /// </summary>
        private static Vector2 GetTargetSize(Image image, RectTransform targetRect)
        {
            if (targetRect != null)
            {
                // Use specified target rect
                return targetRect.rect.size;
            }

            // Find canvas
            Canvas canvas = image.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("[ImageExtension] No Canvas found! Using default size.");
                return UGUIConst.CANVAS_SIZE_MOBILE;
            }

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // Handle different render modes
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    // Overlay: Use canvas rect size directly
                    return canvasRect.rect.size;

                case RenderMode.ScreenSpaceCamera:
                    // Camera: Canvas size is already scaled correctly by Canvas Scaler
                    // Use rect.size which gives us the reference resolution size
                    return canvasRect.rect.size;

                case RenderMode.WorldSpace:
                    // WorldSpace: Use canvas rect size
                    return canvasRect.rect.size;

                default:
                    return canvasRect.rect.size;
            }
        }

        /// <summary>
        /// Fits the Image using explicit scale mode selection
        /// </summary>
        public static void FitImageToRectTransformByRatio(
            this Image image,
            RectTransform targetRect = null,
            ScaleMode scaleMode = ScaleMode.SCALE_TO_FIT)
        {
            if (image == null || image.sprite == null)
            {
                Debug.LogWarning("[ImageExtension] Image or sprite is null!");
                return;
            }

            RectTransform imageRect = image.rectTransform;

            // Get dimensions
            float imageWidth = image.sprite.rect.width;
            float imageHeight = image.sprite.rect.height;

            Vector2 targetSize = GetTargetSize(image, targetRect);
            float targetWidth = targetSize.x;
            float targetHeight = targetSize.y;

            // Calculate ratios
            float widthRatio = targetWidth / imageWidth;
            float heightRatio = targetHeight / imageHeight;

            float finalWidth;
            float finalHeight;

            switch (scaleMode)
            {
                case ScaleMode.SCALE_TO_FIT:
                    // Choose smaller ratio (fit inside)
                    float fitRatio = Mathf.Min(widthRatio, heightRatio);
                    finalWidth = imageWidth * fitRatio;
                    finalHeight = imageHeight * fitRatio;
                    break;

                case ScaleMode.SCALE_TO_CROP:
                    // Choose larger ratio (fill completely)
                    float cropRatio = Mathf.Max(widthRatio, heightRatio);
                    finalWidth = imageWidth * cropRatio;
                    finalHeight = imageHeight * cropRatio;
                    break;

                case ScaleMode.STRETCH_TO_FILL:
                    // Stretch to fill (ignore aspect)
                    finalWidth = targetWidth;
                    finalHeight = targetHeight;
                    break;

                case ScaleMode.USE_WIDTH_RATIO:
                    // Always use width ratio
                    finalWidth = targetWidth;
                    finalHeight = imageHeight * widthRatio;
                    break;

                case ScaleMode.USE_HEIGHT_RATIO:
                    // Always use height ratio
                    finalWidth = imageWidth * heightRatio;
                    finalHeight = targetHeight;
                    break;

                default:
                    finalWidth = imageWidth;
                    finalHeight = imageHeight;
                    break;
            }

            imageRect.sizeDelta = new Vector2(finalWidth, finalHeight);
            imageRect.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Get the recommended scale ratio with detailed canvas info
        /// </summary>
        public static ScaleRatioInfo GetRecommendedScaleRatio(
            this Image image,
            RectTransform targetRect = null)
        {
            if (image == null || image.sprite == null)
                return default;

            float imageWidth = image.sprite.rect.width;
            float imageHeight = image.sprite.rect.height;

            Vector2 targetSize = GetTargetSize(image, targetRect);
            float targetWidth = targetSize.x;
            float targetHeight = targetSize.y;

            float widthRatio = targetWidth / imageWidth;
            float heightRatio = targetHeight / imageHeight;

            // Get canvas info for debugging
            Canvas canvas = image.GetComponentInParent<Canvas>();
            RenderMode renderMode = canvas != null ? canvas.renderMode : RenderMode.ScreenSpaceOverlay;
            Vector2 canvasSize = canvas != null ? canvas.GetComponent<RectTransform>().rect.size : Vector2.zero;

            return new ScaleRatioInfo
            {
                imageSize = new Vector2(imageWidth, imageHeight),
                targetSize = new Vector2(targetWidth, targetHeight),
                canvasSize = canvasSize,
                canvasRenderMode = renderMode,
                widthRatio = widthRatio,
                heightRatio = heightRatio,
                recommendedForContain = Mathf.Min(widthRatio, heightRatio),
                recommendedForCover = Mathf.Max(widthRatio, heightRatio),
                useWidthRatio = widthRatio < heightRatio
            };
        }

        /// <summary>
        /// Get actual screen size (useful for ScreenSpace-Camera debugging)
        /// </summary>
        public static Vector2 GetActualScreenSize(this Image image)
        {
            Canvas canvas = image.GetComponentInParent<Canvas>();
            if (canvas == null) return Vector2.zero;

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
            {
                // For ScreenSpace-Camera, return actual screen resolution
                return new Vector2(Screen.width, Screen.height);
            }

            return canvas.GetComponent<RectTransform>().rect.size;
        }

        /// <summary>
        /// Get reference resolution from Canvas Scaler (for ScreenSpace-Camera)
        /// </summary>
        public static Vector2 GetReferenceResolution(this Image image)
        {
            Canvas canvas = image.GetComponentInParent<Canvas>();
            if (canvas == null) return Vector2.zero;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                return scaler.referenceResolution;
            }

            return canvas.GetComponent<RectTransform>().rect.size;
        }
    }

    /// <summary>
    /// Image fit modes
    /// </summary>
    public enum ImageFitMode
    {
        CONTAIN = 0,    // Fit inside (entire image visible, may have empty space)
        COVER = 1,      // Fill completely (may crop image, no empty space)
        STRETCH = 2     // Stretch to fill (ignore aspect ratio)
    }

    /// <summary>
    /// Scale modes
    /// </summary>
    public enum ScaleMode
    {
        SCALE_TO_FIT = 0,      // Fit inside (like CSS contain)
        SCALE_TO_CROP = 1,     // Fill completely (like CSS cover)
        STRETCH_TO_FILL = 2,   // Stretch to fill
        USE_WIDTH_RATIO = 3,   // Always scale by width
        USE_HEIGHT_RATIO = 4   // Always scale by height
    }

    /// <summary>
    /// Scale ratio information with canvas details
    /// </summary>
    public struct ScaleRatioInfo
    {
        public Vector2 imageSize;
        public Vector2 targetSize;
        public Vector2 canvasSize;
        public RenderMode canvasRenderMode;
        public float widthRatio;
        public float heightRatio;
        public float recommendedForContain;
        public float recommendedForCover;
        public bool useWidthRatio;

        public override string ToString()
        {
            return $"[{canvasRenderMode}] Image: {imageSize}, Target: {targetSize}, Canvas: {canvasSize}\n" +
                   $"Width Ratio: {widthRatio:F3}, Height Ratio: {heightRatio:F3}\n" +
                   $"Contain: {recommendedForContain:F3}, Cover: {recommendedForCover:F3}\n" +
                   $"Recommended: Use {(useWidthRatio ? "Width" : "Height")} Ratio";
        }
    }

    // ============================================================
// USAGE EXAMPLES FOR SCREENSPACE-CAMERA
// ============================================================
public class ScreenSpaceCameraExamples : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image avatarImage;
    [SerializeField] private RectTransform avatarContainer;

    // ============================================================
    // EXAMPLE 1: Background Image (ScreenSpace-Camera)
    // ============================================================
    void SetupBackgroundForCamera()
    {
        // Canvas Setup:
        // - Render Mode: ScreenSpace-Camera
        // - Reference Resolution: 1920x1080
        // - Match: 0.5 (balance width/height)
        
        // Background image will fill the reference resolution
        backgroundImage.FitImageToRectTransformScreenSpaceCamera(null, ImageFitMode.COVER);
        
        // Result: Image fills 1920x1080 (reference resolution)
        // Actual screen (e.g., 2560x1440) is handled by Canvas Scaler
    }

    // ============================================================
    // EXAMPLE 2: Debug Canvas Info
    // ============================================================
    void DebugCanvasInfo()
    {
        var info = backgroundImage.GetRecommendedScaleRatio();
        Debug.Log(info.ToString());
        
        // Example Output:
        // [ScreenSpaceCamera] Image: (2048, 1536), Target: (1920, 1080), Canvas: (1920, 1080)
        // Width Ratio: 0.938, Height Ratio: 0.703
        // Contain: 0.703, Cover: 0.938
        // Recommended: Use Height Ratio
        
        // Additional info
        Vector2 screenSize = backgroundImage.GetActualScreenSize();
        Vector2 refResolution = backgroundImage.GetReferenceResolution();
        
        Debug.Log($"Screen: {screenSize}, Reference: {refResolution}");
        // Output: Screen: (2560, 1440), Reference: (1920, 1080)
    }

    // ============================================================
    // EXAMPLE 3: Avatar in Fixed Container
    // ============================================================
    void SetupAvatarImage()
    {
        // Avatar container: 200x200 (square)
        // Avatar image: 512x512 (square photo)
        
        // Fill the container (crop to circle later with mask)
        avatarImage.FitImageToRectTransformScreenSpaceCamera(avatarContainer, ImageFitMode.COVER);
        
        // Result: 200x200 (perfect fit for square images)
    }

    // ============================================================
    // EXAMPLE 4: Responsive Background
    // ============================================================
    void SetupResponsiveBackground()
    {
        // This works across different aspect ratios:
        // - Phone (9:16): 1080x1920
        // - Tablet (4:3): 1536x2048
        // - Desktop (16:9): 1920x1080
        
        backgroundImage.FitImageToRectTransformScreenSpaceCamera(null, ImageFitMode.COVER);
        
        // Canvas Scaler handles the scaling automatically
        // Image always fills the reference resolution
    }

    // ============================================================
    // EXAMPLE 5: Handle Different Screen Sizes
    // ============================================================
    void Start()
    {
        // Get current setup info
        var info = backgroundImage.GetRecommendedScaleRatio();
        
        Debug.Log($"Canvas Render Mode: {info.canvasRenderMode}");
        Debug.Log($"Canvas Size (Reference): {info.canvasSize}");
        Debug.Log($"Image Size: {info.imageSize}");
        Debug.Log($"Screen Size: {backgroundImage.GetActualScreenSize()}");
        
        // Fit background
        backgroundImage.FitImageToRectTransformScreenSpaceCamera(null, ImageFitMode.COVER);
    }
}

// ============================================================
// CANVAS SCALER COMPATIBILITY NOTES
// ============================================================
/*
SCREENSPACE-CAMERA SETUP:

Canvas Settings:
├── Render Mode: Screen Space - Camera
├── Render Camera: [Main Camera]
├── Plane Distance: 10
└── Canvas Scaler:
    ├── UI Scale Mode: Scale With Screen Size
    ├── Reference Resolution: 1920 x 1080
    ├── Screen Match Mode: Match Width Or Height
    └── Match: 0.5

HOW IT WORKS:

1. Reference Resolution (1920x1080):
   - Canvas.rect.size returns reference resolution
   - NOT the actual screen size
   
2. Canvas Scaler handles scaling:
   - Actual screen: 2560x1440
   - Canvas shows: 1920x1080 (reference)
   - Scaler auto-scales to fit
   
3. Our FitImageToRectTransform:
   - Uses canvas.rect.size (reference resolution)
   - Works correctly with Canvas Scaler
   - No need to manually calculate scaling

EXAMPLE FLOW:

Screen:     2560x1440 (actual device)
            ↓ [Canvas Scaler]
Reference:  1920x1080 (what we work with)
            ↓ [FitImageToRectTransform]
Image:      Scaled to fit 1920x1080
            ↓ [Canvas Scaler applies to screen]
Result:     Image fills 2560x1440 correctly

DIFFERENT ASPECT RATIOS:

Phone (9:16):
- Screen: 1080x1920
- Reference: 1920x1080 (Canvas Scaler adjusts)
- Match: 0.5 (balance width/height)
- Image fits reference, Scaler handles rest

Tablet (4:3):
- Screen: 1536x2048
- Reference: 1920x1080
- Match: 0.5
- Image fits reference, Scaler handles rest

Desktop (16:9):
- Screen: 1920x1080
- Reference: 1920x1080 (perfect match!)
- No scaling needed
*/

// ============================================================
// DEBUGGING HELPER
// ============================================================
public class CanvasDebugger : MonoBehaviour
{
    [SerializeField] private Image testImage;
    
    [ContextMenu("Debug Canvas Info")]
    void DebugInfo()
    {
        Canvas canvas = testImage.GetComponentInParent<Canvas>();
        
        Debug.Log("=== Canvas Debug Info ===");
        Debug.Log($"Render Mode: {canvas.renderMode}");
        Debug.Log($"Canvas Rect Size: {canvas.GetComponent<RectTransform>().rect.size}");
        Debug.Log($"Screen Size: {new Vector2(Screen.width, Screen.height)}");
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler)
        {
            Debug.Log($"Reference Resolution: {scaler.referenceResolution}");
            Debug.Log($"Scale Mode: {scaler.uiScaleMode}");
            Debug.Log($"Match: {scaler.matchWidthOrHeight}");
        }
        
        Debug.Log("\n=== Image Scale Info ===");
        var info = testImage.GetRecommendedScaleRatio();
        Debug.Log(info.ToString());
    }
}
    
    
}

