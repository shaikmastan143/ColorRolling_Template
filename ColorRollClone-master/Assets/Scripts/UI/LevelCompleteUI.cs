using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] MainGame mainGame;
    [SerializeField] Text mainText;
    [SerializeField] Button okButton;


    private Animator animator;

    private int animationHashCode_Show;
    private int animationHashCode_Hide;

    public Action Done = delegate { };

    public Action PlayButtonClicked = delegate { };

    private void Awake()
    {
        Hide();

        animator = GetComponent<Animator>();

        animationHashCode_Show = Animator.StringToHash("show");
        animationHashCode_Hide = Animator.StringToHash("hide");
    }


    public void HandleLevelCompleted()
    {
        mainText.text = "SPECIAL LEVEL\nUNLOCKED!";

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        animator.SetTrigger(animationHashCode_Show);
    }

    public void Hide()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationHashCode_Hide);

            float t = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            new DelayAction(this, () => {

                gameObject.SetActive(false);

            }, t);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void HandleOkButtonClicked()
    {
        Hide();

        Done?.Invoke();
    }

    public void HandlePlayButtonClicked()
    {
        Hide();

        Done?.Invoke();

        PlayButtonClicked?.Invoke();
    }
}
