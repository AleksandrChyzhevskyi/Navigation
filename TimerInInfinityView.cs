using System;
using _Development.Scripts.Extensions;
using TMPro;
using UnityEngine;

public class TimerInInfinityView : MonoBehaviour
{
    public TextMeshProUGUI Timer;

    public void Show(TimeSpan time = default) => 
        Timer.text = time != default ? time.TimeSpanToString() : "PREPARING...";
}