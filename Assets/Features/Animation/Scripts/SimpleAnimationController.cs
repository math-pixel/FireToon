using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleAnimationController : MonoBehaviour
{
    private Animator animator;
    private AnimationClipData current;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(AnimationClipData clip)
    {
        if (clip == null || current == clip) return;

        animator.CrossFade(clip.animationName, clip.transitionDuration);
        current = clip;
    }
}