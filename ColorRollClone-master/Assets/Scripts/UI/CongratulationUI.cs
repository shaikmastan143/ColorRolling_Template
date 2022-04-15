using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationUI : MonoBehaviour
{
    private Animator animator;

    private int hashCode_Show;
    private int hashCode_Hide;

    void Start()
    {
        HideImmediate();

        animator = GetComponent<Animator>();

        hashCode_Show = Animator.StringToHash("show");
        hashCode_Hide = Animator.StringToHash("hide");
    }

    public void Show()
    {
        gameObject.SetActive(true);

        animator.SetTrigger(hashCode_Show);

        SoundController.Current.PlayPopup();

    }
    public void Hide()
    {
        SoundController.Current.PlayPopup();

        animator.SetTrigger(hashCode_Hide);

        float t = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        new DelayAction(this, () => {

            gameObject.SetActive(false);

        }, t);
    }
    public void HideImmediate()
    {
        gameObject.SetActive(false);
    }
}
