using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI Animations")]
    [Header("Logo")]
    public Transform logo;
    public Vector3 firstSizePulse, secondSizePulse;

    [Header("Buttons")]
    public RectTransform ButtonPlay;
    public RectTransform ButtonSettings;
    public Vector3 EndButtonPlay, EndButtonSettings;
    
    // Start is called before the first frame update
    void Start()
    {
        logo.DOScale(firstSizePulse, 2f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                logo.DOScale(secondSizePulse, 2f)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        ButtonPlay.DOAnchorPos(EndButtonPlay, 2f)
            .SetEase(Ease.OutBounce)
            .SetDelay(1f);
        
        ButtonSettings.DOAnchorPos(EndButtonSettings, 2f)
            .SetEase(Ease.OutBounce)
            .SetDelay(1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void changeScene()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.Lobby);
    }
}
