using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimerScript : MonoBehaviour
{
    public static TimerScript instance;
    public static event System.Action timerOver = delegate { };

    [SerializeField] private Image[] timers;

    Sequence flashing;

    private void Awake()
    {
        instance = this;
    }
    public void StartTimer(float time, int player)
    {
        if (!timers[player].gameObject.activeInHierarchy)
        {
            Debug.Log("Starting timer for player" + player);
            flashing = DOTween.Sequence();
            flashing.SetAutoKill(true);
            for (int i = 0; i <= (int)(time / 2f); i++)
            {
                flashing.Append(timers[player].DOFade(0.1f, 0.3f).From(1f).SetEase(Ease.InQuart));
                flashing.Append(timers[player].DOFade(1f, 0.7f).SetEase(Ease.OutQuart));
            }
            flashing.SetDelay(time / 2f).PlayForward();
            timers[player].gameObject.SetActive(true);
            timers[player].DOFillAmount(0f, time).From(1f).SetEase(Ease.Linear).OnComplete(TimeOver);
        }
    }

    private void TimeOver()
    {
        Debug.Log("Timer Over");
        StopTimer();
        timerOver();
        
    }

    public void StopTimer()
    {
        Debug.Log("Stopping Timer");
        timers[0].DOKill();
        timers[1].DOKill();
        timers[0].fillAmount = 0f;
        timers[1].fillAmount = 0f;
        flashing.Kill();
        timers[0].gameObject.SetActive(false);
        timers[1].gameObject.SetActive(false);
    }
}
