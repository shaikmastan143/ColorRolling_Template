using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    [SerializeField] private float time = 1.0f;

    private Animator animator;

    private void Awake()
    {

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(loadScene());
    }

    IEnumerator loadScene()
    {
        var o = SceneManager.LoadSceneAsync("GameScene");

        o.allowSceneActivation = false;


        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            progressBar.fillAmount = t/time;

            yield return null;
        }


        yield return new WaitForSeconds(0.9f);

        animator.SetTrigger("hide");

        o.allowSceneActivation = true;

        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[0].length);

        gameObject.SetActive(false);
    }
}
