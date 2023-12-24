using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    // private TextMeshProUGUI textMeshProComponent;
    public TMP_Text timerText;
    private DateTime startTime;
    private DateTime curTime;
    private int timeOverAllSeconds = 120;
    void Start()
    {
        startTime = DateTime.Now;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        curTime = DateTime.Now;
        TimeSpan timeDifference = curTime.Subtract(startTime);
        int secsRemaining = timeOverAllSeconds - 60 * timeDifference.Minutes - timeDifference.Seconds;
        int minsRemaining = secsRemaining / 60;
        String secs = (secsRemaining % 60).ToString();
        if ((secsRemaining % 60) < 10) {
            secs = "0" + secs;
        }
        if (secsRemaining < 0) {
            timerText.text = "Lost";
            return;
        }
        timerText.text = minsRemaining.ToString() + ":" + secs;
    }
}
