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
    public DateTime startTime;
    private DateTime curTime;
    private int timeOverAllSeconds = 120;

    private int lostTimeEnded = -10;
    [SerializeField] private Player mainCharacter;

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
            timerText.text = "Lost! You are dead. Start from the starting location.";
            if (secsRemaining <= lostTimeEnded)
            {
                mainCharacter.Restart();
                startTime = DateTime.Now;
            }
            return;
        }
        timerText.text = minsRemaining.ToString() + ":" + secs;
    }

    IEnumerator BlockMovementForDuration(float blockDuration)
    {
        yield return new WaitForSeconds(blockDuration);
    }
}
