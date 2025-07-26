using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UIAnimationConfig", menuName = "Game/UI Animation Config")]
public class UIAnimationConfig : ScriptableObject
{
    [System.Serializable]
    public class AnimationElement
    {
        [Header("Element Info")]
        public string elementName;
        public AnimationType type;
        
        [Header("Timing")]
        public float delay = 0f;
        public float duration = 1f;
        public Ease ease = Ease.OutQuart;
        
        [Header("Movement")]
        public Vector3 fromPosition;
        public Vector3 toPosition;
        public bool useLocalPosition = true;
        
        [Header("Scale")]
        public Vector3 fromScale = Vector3.one;
        public Vector3 toScale = Vector3.one;
        
        [Header("Rotation")]
        public Vector3 fromRotation;
        public Vector3 toRotation;
        
        [Header("Fade")]
        public float fromAlpha = 1f;
        public float toAlpha = 1f;
        
        [Header("Loop Settings")]
        public bool loop = false;
        public LoopType loopType = LoopType.Yoyo;
        public int loopCount = -1;
        
        [Header("Special Effects")]
        public bool punch = false;
        public Vector3 punchStrength = Vector3.one * 0.1f;
        public bool shake = false;
        public Vector3 shakeStrength = Vector3.one * 0.1f;
    }
    
    public enum AnimationType
    {
        Move,
        Scale,
        Rotate,
        Fade,
        MoveAndScale,
        MoveAndFade,
        ScaleAndFade,
        All,
        Punch,
        Shake
    }
    
    [Header("Animation Sequence")]
    public List<AnimationElement> animationElements = new List<AnimationElement>();
    
    [Header("Global Settings")]
    public bool playOnStart = true;
    public bool autoCleanup = true;
    public float globalTimeScale = 1f;
    
    [Header("Logo Animation")]
    public Vector3 logoFirstScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 logoSecondScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float logoFirstDuration = 2f;
    public float logoSecondDuration = 2f;
    public Ease logoEase = Ease.OutBounce;
    
    [Header("Button Animation")]
    public float buttonAnimationDuration = 2f;
    public float buttonDelayIncrement = 0.2f;
    public Ease buttonEase = Ease.OutBounce;
    
    [Header("Ending Animation")]
    public float replayButtonDuration = 2f;
    public float replayButtonDelay = 2f;
    public Vector3 replayButtonScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float replayScaleDuration = 5f;
    public Ease replayEase = Ease.OutBack;
}
