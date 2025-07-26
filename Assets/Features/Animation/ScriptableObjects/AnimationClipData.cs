using UnityEngine;

[CreateAssetMenu(menuName = "SimpleAnimation/Animation Clip")]
public class AnimationClipData : ScriptableObject
{
    public string animationName;
    public float transitionDuration = 0.25f;
}