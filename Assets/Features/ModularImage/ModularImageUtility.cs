#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Features.ModularImage
{
    public static class ModularImageUtility
    {
        [MenuItem("CONTEXT/Image/Replace with Procedural Image")]
        private static void ReplaceImageWithProcedural(MenuCommand command)
        {
            Image image = (Image)command.context;
            ReplaceWithProceduralImage(image.gameObject);
        }
    
        [MenuItem("GameObject/UI/Procedural Image", false, 2001)]
        private static void CreateProceduralImage()
        {
            GameObject go = new GameObject("Procedural Image");
        
            if (Selection.activeGameObject != null)
            {
                Canvas canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    go.transform.SetParent(Selection.activeGameObject.transform, false);
                }
            }
        
            var rectTransform = go.AddComponent<RectTransform>();
            var proceduralImage = go.AddComponent<ModularImage>();
        
            rectTransform.sizeDelta = new Vector2(100, 100);
            proceduralImage.color = Color.white;
        
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Procedural Image");
        }
    
        public static void ReplaceWithProceduralImage(GameObject gameObject)
        {
            Image existingImage = gameObject.GetComponent<Image>();
            if (existingImage == null) return;
        
            var savedProperties = new ImageProperties(existingImage);
        
            Undo.RecordObject(gameObject, "Replace with Procedural Image");
            Undo.DestroyObjectImmediate(existingImage);
        
            var proceduralImage = Undo.AddComponent<ModularImage>(gameObject);
            savedProperties.ApplyTo(proceduralImage);
        
            Debug.Log($"Replaced Image with ProceduralUIImage on {gameObject.name}");
        }
    
        private class ImageProperties
        {
            public Color color;
            public Sprite sprite;
            public Material material;
            public bool raycastTarget;
            public Vector4 raycastPadding;
            public bool maskable;
            public Image.Type type;
            public bool preserveAspect;
            public bool useSpriteMesh;
            public bool fillCenter;
            public float pixelsPerUnitMultiplier;
            public Image.FillMethod fillMethod;
            public float fillAmount;
            public bool fillClockwise;
            public int fillOrigin;
        
            public ImageProperties(Image image)
            {
                color = image.color;
                sprite = image.sprite;
                material = image.material;
                raycastTarget = image.raycastTarget;
                raycastPadding = image.raycastPadding;
                maskable = image.maskable;
                type = image.type;
                preserveAspect = image.preserveAspect;
                useSpriteMesh = image.useSpriteMesh;
                fillCenter = image.fillCenter;
                pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
                fillMethod = image.fillMethod;
                fillAmount = image.fillAmount;
                fillClockwise = image.fillClockwise;
                fillOrigin = image.fillOrigin;
            }
        
            public void ApplyTo(ModularImage proceduralImage)
            {
                proceduralImage.color = color;
                proceduralImage.sprite = sprite;
                proceduralImage.material = material;
                proceduralImage.raycastTarget = raycastTarget;
                proceduralImage.raycastPadding = raycastPadding;
                proceduralImage.maskable = maskable;
                proceduralImage.type = type;
                proceduralImage.preserveAspect = preserveAspect;
                proceduralImage.useSpriteMesh = useSpriteMesh;
                proceduralImage.fillCenter = fillCenter;
                proceduralImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
                proceduralImage.fillMethod = fillMethod;
                proceduralImage.fillAmount = fillAmount;
                proceduralImage.fillClockwise = fillClockwise;
                proceduralImage.fillOrigin = fillOrigin;
            
                proceduralImage.UseProceduralShape = true;
                proceduralImage.UniformRadius = 5f;
            }
        }
    }
    
    // Extensions for fluid usage
    public static class ProceduralUIImageExtensions
    {
        public static ModularImage SetRoundedCorners(this ModularImage image, float radius)
        {
            image.UniformRadius = radius;
            return image;
        }
    
        public static ModularImage SetOutline(this ModularImage image, float width, Color color)
        {
            image.SetOutline(width, color);
            return image;
        }
    
        public static ModularImage SetColor(this ModularImage image, Color color)
        {
            image.color = color;
            return image;
        }
    }
#endif
}