using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetAlpha(1f);
        StartCoroutine(FadeIn(1f));
    }

    // ボタンから呼び出す
    public void StartTransition()
    {
        StartCoroutine(Transition());
    }

    // シーン遷移
    private IEnumerator Transition()
    {
        // フェードアウト完了まで待つ
        yield return StartCoroutine(FadeOut(1f));

        // シーン移動
        SceneManager.LoadScene("Main");

        // シーンが切り替わるまで1フレーム待つ
        yield return null;

        // フェードイン
        yield return StartCoroutine(FadeIn(1f));
    }

    // フェードアウト
    private IEnumerator FadeOut(float duration)
    {
        yield return StartCoroutine(Fade(1f, duration));
    }

    // フェードイン
    private IEnumerator FadeIn(float duration)
    {
        yield return StartCoroutine(Fade(0f, duration));
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image が設定されていません！");
            yield break;
        }

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image が設定されていません！");
            return;
        }

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}