using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine;

public class EndingManager : MonoBehaviour
{

    public RectTransform ReplayButton;
    public Vector3 ButtonEndPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        ReplayButton.DOAnchorPos(ButtonEndPosition, 2f)
            .SetEase(Ease.OutElastic)
            .SetDelay(2f)
            .OnComplete(() =>
            {
                ReplayButton.DOScale(1.5f, 5f)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
