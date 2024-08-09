using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private float scoreSpeed;
    Tween closeTextTween;
    private bool shown = false, isDrifting = false;

    CarSynchronizer localCarSynchronizer;

    private float currentDriftScore, totalScore;

    private void Awake()
    {
        CarController.AnnounceLocalPlayer.AddListener(OnAnnounceLocalPlayer);
        gameObject.SetActive(false);
    }

    private void OnAnnounceLocalPlayer(CarController localCar)
    {
        localCarSynchronizer = localCar.GetComponent<CarSynchronizer>();
        localCar.OnDriftStart.AddListener(OnDriftStart);
        localCar.OnDriftEnd.AddListener(OnDriftEnd);
    }

    private void Update()
    {
        if (!isDrifting)
        {
            return;
        }

        float addedScore = Time.deltaTime * scoreSpeed;
        currentDriftScore += addedScore;
        totalScore += addedScore;
        localCarSynchronizer.SetScore((int)totalScore);
        scoreText.text = "x" + ((int)currentDriftScore);
    }

    private void OnDriftStart()
    {
        isDrifting = true;
        if (shown)
        {
            closeTextTween?.Kill();
            return;
        }

        Show();
    }

    private void OnDriftEnd()
    {
        isDrifting = false;
        if (!shown)
        {
            return;
        }

        closeTextTween?.Kill();
        closeTextTween = DOVirtual.DelayedCall(2f, Hide);
    }

    private void Show()
    {
        shown = true;
        gameObject.SetActive(true);
        transform.DOKill();
        currentDriftScore = 0;
        transform.DOScale(Vector3.one, .2f);
    }

    private void Hide()
    {
        shown = false;
        transform.DOKill();
        transform.DOScale(Vector3.zero, .2f).OnComplete(() => gameObject.SetActive(false));
    }
}
