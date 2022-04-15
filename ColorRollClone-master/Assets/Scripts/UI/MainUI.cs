using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject hintParticlePrefab;
    [SerializeField] private GameObject hintParticleSpawningPosition;

    [SerializeField] private GameDataManager gameDataManager;

    [SerializeField] public HintUI hintUI;

    [SerializeField] private Image smilyFace;

    [SerializeField] private MainGame mainGame;
    [SerializeField] private CongratulationUI congratulation;

    [SerializeField] private GameObject lockIcon;

    public Action HintButtonClicked = delegate { };

    private int hashCode_LockShow;



    private void Awake()
    {
        mainGame.HintNumChanged += hintUI.UpdateHintButton;
        mainGame.LevelCompleted += HandleLevelCompleted;
        mainGame.NewLevelLoaded += _=>Reset();
        mainGame.AllLevelsClear += ShowCongratulation;
        mainGame.TouchedOnLockedCarpet += ShowLock;

        smilyFace.gameObject.SetActive(false);
        lockIcon.SetActive(false);

        hintUI.UpdateHintButton(mainGame.gameDataManager.GameDataSO.HintNum);

        hashCode_LockShow = Animator.StringToHash("show");
    }


    private void Reset()
    {
        hintUI.UpdateHintButton(mainGame.gameDataManager.GameDataSO.HintNum);

        hintUI.gameObject.SetActive(true);

        smilyFace.gameObject.SetActive(false);

        congratulation.HideImmediate();// SetActive(false);

    }

    private void HandleLevelCompleted(int level)
    {
        hintUI.gameObject.SetActive(false);

        ShowSmileyFace();

        congratulation.HideImmediate();
    }

    public void ShowCongratulation()
    {
        hintUI.gameObject.SetActive(false);

        smilyFace.gameObject.SetActive(false);

        congratulation.Show();
    }


    public void OnHintButtonClicked()
    {
        HintButtonClicked?.Invoke();

        if (gameDataManager.GameDataSO.HintNum <=0)
        {
            hintUI.ShakeHintButton();
        }
    }


    public void ShowSmileyFace()
    {
        smilyFace.gameObject.SetActive(true);

        var animator = smilyFace.GetComponent<Animator>();

        animator?.SetTrigger("show");
    }

    public void ShowLock()
    {
        //if (!lockAnimator.GetCurrentAnimatorStateInfo(0).IsName("popup_appear"))
        //{
        //    lockAnimator.SetTrigger(hashCode_LockShow);
        //}
        if (!lockIcon.activeSelf)
        {
            lockIcon.SetActive(true);

            new DelayAction(this, () => { lockIcon.SetActive(false); }, 1.0f);
        }
    }

    public void HandleResetButtonClicked()
    {
        mainGame.ResetGame();
    }
    //public void HandleShareButtonClicked()
    //{
    //}
    //public void HandleRatingButtonClicked()
    //{
    //}

    public void SpawnHintParticle(int n)
    {
        for(int i = 0; i<n; i++)
        {
            new DelayAction(this, () => {

                var hintParticle = Instantiate(hintParticlePrefab, transform);

                hintParticle.transform.position = hintParticleSpawningPosition.transform.position;

                hintParticle.GetComponent<HintParticle>().target = hintUI.gameObject;

                hintParticle.GetComponent<HintParticle>().Disappear += () =>
                {
                    hintUI.UpdateHintButton(hintUI.rememberedHintNum + 1);

                    hintUI.ShakeHintButton();
                };

            }, i * 0.25f);
        }
    }
}
