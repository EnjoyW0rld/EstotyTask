using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    //*************************************************************************
    //    Class aimed for both calculating and drawing time on screen.
    //    Is subscribed to ball "complete" event and calcutes outcomes of
    //    where it fall
    //*************************************************************************

    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _timerText;
        [SerializeField] private float _maxTime = 30;
        private bool _elapsed;
        private float _currentTime;

        GameManager _manager => GameManager.Instance;
        private void Start()
        {
            _manager.OnBallCompleted.AddListener(OnBallCompleted);
            _manager.OnGameStarted.AddListener(RestartTimer);
        }
        private void Update()
        {
            if (_elapsed || !_manager.IsStarted) return;
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                _timerText.text = (int)_currentTime + "";
            }
            else
            {
                GameManager.Instance.OnTimerEnded();
                _timerText.text = "Over";
                _elapsed = true;
            }
        }
        private void RestartTimer()
        {
            _currentTime = _maxTime;
            _elapsed = false;
        }
        private void OnBallCompleted(GameManager.CompletionType pType)
        {
            switch (pType)
            {
                case GameManager.CompletionType.Bad:
                    SubstractTime(_manager.BadHoleTimeSub);
                    break;
                case GameManager.CompletionType.Good:
                    AddTime(_manager.GoodHoleTimeAdd);
                    break;
            }
        }
        public void AddTime(float pTime) => _currentTime += pTime;
        public void SubstractTime(float pTime) => _currentTime -= pTime;
    }
}