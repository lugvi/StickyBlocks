using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using GameAnalyticsSDK;
using System;

public class AnalyticsManager : MonoBehaviour
{


    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        GameAnalytics.Initialize();

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

    }

#region FACEBOOK

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
#endregion


#region GAME_ANALYTICS

    // es punqcia gamoidzaxe wagebis dros
    public void GameOverEvent(float score)
    {
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", (int)score);
    }

    // es punqcia gamoidzaxe dawyebis dros
    public void StartEvent()
    {
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
    }

    public void HighScoreEvent(int score)
    {
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "highscore",(int)score);
    }


    #endregion

}
