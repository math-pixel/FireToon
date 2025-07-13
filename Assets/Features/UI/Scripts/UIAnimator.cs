using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class UIAnimator : MonoBehaviour
{
    [Header("Configuration")]
    public UIAnimationConfig animationConfig;
    
    [Header("Auto-Detection")]
    public bool autoDetectElements = true;
    public string[] elementTags = { "Logo", "Button", "Panel", "Text" };
    
    [Header("Manual References")]
    public List<UIElement> manualElements = new List<UIElement>();
    
    [System.Serializable]
    public class UIElement
    {
        public string name;
        public Transform transform;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public Vector3 originalPosition;
        public Vector3 originalScale;
        public Vector3 originalRotation;
        public float originalAlpha;
    }
    
    private Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();
    private Sequence mainSequence;
    private List<Tween> activeTweens = new List<Tween>();
    
    void Start()
    {
        InitializeElements();
        
        if (animationConfig != null && animationConfig.playOnStart)
        {
            PlayAnimations();
        }
    }
    
    private void InitializeElements()
    {
        elements.Clear();
        
        // Auto-detect elements
        if (autoDetectElements)
        {
            AutoDetectUIElements();
        }
        
        // Add manual elements
        foreach (var element in manualElements)
        {
            if (!elements.ContainsKey(element.name))
            {
                StoreOriginalValues(element);
                elements.Add(element.name, element);
            }
        }
    }
    
    private void AutoDetectUIElements()
    {
        // Detect by tags
        foreach (string tag in elementTags)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in taggedObjects)
            {
                if (obj.transform.IsChildOf(transform) || obj.transform == transform)
                {
                    CreateUIElement(obj, tag);
                }
            }
        }
        
        // Detect by component types
        DetectByComponents();
    }
    
    private void DetectByComponents()
    {
        // Detect buttons
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            CreateUIElement(button.gameObject, "Button");
        }
        
        // Detect images
        Image[] images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            if (image.gameObject.name.ToLower().Contains("logo"))
            {
                CreateUIElement(image.gameObject, "Logo");
            }
        }
        
        // Detect text elements
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            CreateUIElement(text.gameObject, "Text");
        }
    }
    
    private void CreateUIElement(GameObject obj, string type)
    {
        string elementName = $"{type}_{obj.name}";
        
        if (elements.ContainsKey(elementName)) return;
        
        UIElement element = new UIElement
        {
            name = elementName,
            transform = obj.transform,
            rectTransform = obj.GetComponent<RectTransform>(),
            canvasGroup = obj.GetComponent<CanvasGroup>()
        };
        
        // Add CanvasGroup if needed for fade animations
        if (element.canvasGroup == null && HasFadeAnimation(elementName))
        {
            element.canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        
        StoreOriginalValues(element);
        elements.Add(elementName, element);
    }
    
    private void StoreOriginalValues(UIElement element)
    {
        if (element.transform != null)
        {
            element.originalPosition = element.rectTransform != null ? 
                element.rectTransform.anchoredPosition : element.transform.localPosition;
            element.originalScale = element.transform.localScale;
            element.originalRotation = element.transform.localEulerAngles;
        }
        
        if (element.canvasGroup != null)
        {
            element.originalAlpha = element.canvasGroup.alpha;
        }
    }
    
    private bool HasFadeAnimation(string elementName)
    {
        if (animationConfig == null) return false;
        
        foreach (var anim in animationConfig.animationElements)
        {
            if (anim.elementName == elementName && 
                (anim.type == UIAnimationConfig.AnimationType.Fade ||
                 anim.type == UIAnimationConfig.AnimationType.MoveAndFade ||
                 anim.type == UIAnimationConfig.AnimationType.ScaleAndFade ||
                 anim.type == UIAnimationConfig.AnimationType.All))
            {
                return true;
            }
        }
        return false;
    }
    
    public void PlayAnimations()
    {
        if (animationConfig == null) return;
        
        StopAllAnimations();
        
        mainSequence = DOTween.Sequence();
        mainSequence.timeScale = animationConfig.globalTimeScale;
        
        foreach (var animElement in animationConfig.animationElements)
        {
            CreateAnimation(animElement);
        }
        
        if (animationConfig.autoCleanup)
        {
            mainSequence.OnComplete(() => CleanupCompletedTweens());
        }
    }
    
    private void CreateAnimation(UIAnimationConfig.AnimationElement animElement)
    {
        if (!elements.ContainsKey(animElement.elementName))
        {
            Debug.LogWarning($"Element '{animElement.elementName}' not found!");
            return;
        }
        
        UIElement element = elements[animElement.elementName];
        Tween tween = null;
        
        switch (animElement.type)
        {
            case UIAnimationConfig.AnimationType.Move:
                tween = CreateMoveTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.Scale:
                tween = CreateScaleTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.Rotate:
                tween = CreateRotateTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.Fade:
                tween = CreateFadeTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.MoveAndScale:
                tween = CreateMoveAndScaleTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.MoveAndFade:
                tween = CreateMoveAndFadeTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.ScaleAndFade:
                tween = CreateScaleAndFadeTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.All:
                tween = CreateAllTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.Punch:
                tween = CreatePunchTween(element, animElement);
                break;
            case UIAnimationConfig.AnimationType.Shake:
                tween = CreateShakeTween(element, animElement);
                break;
        }
        
        if (tween != null)
        {
            tween.SetEase(animElement.ease);
            
            if (animElement.loop)
            {
                tween.SetLoops(animElement.loopCount, animElement.loopType);
            }
            
            mainSequence.Insert(animElement.delay, tween);
            activeTweens.Add(tween);
        }
    }
    
    private Tween CreateMoveTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        if (element.rectTransform != null)
        {
            element.rectTransform.anchoredPosition = animElement.fromPosition;
            return element.rectTransform.DOAnchorPos(animElement.toPosition, animElement.duration);
        }
        else
        {
            element.transform.localPosition = animElement.fromPosition;
            return element.transform.DOLocalMove(animElement.toPosition, animElement.duration);
        }
    }
    
    private Tween CreateScaleTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        element.transform.localScale = animElement.fromScale;
        return element.transform.DOScale(animElement.toScale, animElement.duration);
    }
    
    private Tween CreateRotateTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        element.transform.localEulerAngles = animElement.fromRotation;
        return element.transform.DOLocalRotate(animElement.toRotation, animElement.duration);
    }
    
    private Tween CreateFadeTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        if (element.canvasGroup == null) return null;
        
        element.canvasGroup.alpha = animElement.fromAlpha;
        return element.canvasGroup.DOFade(animElement.toAlpha, animElement.duration);
    }
    
    private Tween CreateMoveAndScaleTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(CreateMoveTween(element, animElement));
        sequence.Join(CreateScaleTween(element, animElement));
        return sequence;
    }
    
    private Tween CreateMoveAndFadeTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(CreateMoveTween(element, animElement));
        sequence.Join(CreateFadeTween(element, animElement));
        return sequence;
    }
    
    private Tween CreateScaleAndFadeTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(CreateScaleTween(element, animElement));
        sequence.Join(CreateFadeTween(element, animElement));
        return sequence;
    }
    
    private Tween CreateAllTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(CreateMoveTween(element, animElement));
        sequence.Join(CreateScaleTween(element, animElement));
        sequence.Join(CreateRotateTween(element, animElement));
        sequence.Join(CreateFadeTween(element, animElement));
        return sequence;
    }
    
    private Tween CreatePunchTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        return element.transform.DOPunchScale(animElement.punchStrength, animElement.duration);
    }
    
    private Tween CreateShakeTween(UIElement element, UIAnimationConfig.AnimationElement animElement)
    {
        return element.transform.DOShakePosition(animElement.duration, animElement.shakeStrength);
    }
    
    public void StopAllAnimations()
    {
        mainSequence?.Kill();
        
        foreach (var tween in activeTweens)
        {
            tween?.Kill();
        }
        activeTweens.Clear();
    }
    
    public void ResetToOriginal()
    {
        foreach (var element in elements.Values)
        {
            if (element.rectTransform != null)
            {
                element.rectTransform.anchoredPosition = element.originalPosition;
            }
            else
            {
                element.transform.localPosition = element.originalPosition;
            }
            
            element.transform.localScale = element.originalScale;
            element.transform.localEulerAngles = element.originalRotation;
            
            if (element.canvasGroup != null)
            {
                element.canvasGroup.alpha = element.originalAlpha;
            }
        }
    }
    
    private void CleanupCompletedTweens()
    {
        activeTweens.RemoveAll(t => t == null || !t.IsActive());
    }
    
    void OnDestroy()
    {
        StopAllAnimations();
    }
}
