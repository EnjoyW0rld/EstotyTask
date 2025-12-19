using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BallThrowGame
{
    //*************************************************************************
    //  Singleton to hold variables that might be needed in multiple places.
    //  Also serves as a container for events
    //*************************************************************************

    public class GameManager : MonoBehaviour
    {
        [Header("Hole variables")]
        [SerializeField] private int _holePositionChangeCooldown;
        [SerializeField] private int _goodHoleTimeAdd = 5;
        [SerializeField] private int _badHoleTimeSub = 10;
        [Header("Shoot variables")]
        [SerializeField, Tooltip("Position after which the ball will be respawned")] private float _yDeathZone;
        private EasingController _easingController;
        private bool _canShoot;
        //Events
        [Header("Events")]
        public UnityEvent<CompletionType> OnBallCompleted;
        public UnityEvent OnBallShot;
        // Enums
        public enum CompletionType { Bad, Neutral, Good }

        // Getter fields
        public int HolePositionChangeCooldown => _holePositionChangeCooldown;
        public EasingController EasingController => _easingController;
        public float YDeathZone => _yDeathZone;
        public bool CanShoot => _canShoot;
        public int GoodHoleTimeAdd => _goodHoleTimeAdd;
        public int BadHoleTimeSub => _badHoleTimeSub;

        //Singlton variables
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            _easingController = FindObjectOfType<EasingController>();
        }
        private void Start()
        {
            if (OnBallCompleted == null) OnBallCompleted = new UnityEvent<CompletionType>();
            if (OnBallShot == null) OnBallShot = new UnityEvent();
        }

        /// <summary>
        /// Call when ball fell into one of the holes or out of the map,
        /// "completed" its journey
        /// </summary>
        /// <param name="pType"></param>
        public void CallBallComplete(CompletionType pType)
        {
            _canShoot = true;
            OnBallCompleted?.Invoke(pType);
            switch (pType)
            {
                case CompletionType.Bad:
                    break;
                case CompletionType.Good:
                    break;
            }
        }
        /// <summary>
        /// Call when ball was just shot
        /// </summary>
        public void CallBallShot()
        {
            _canShoot = false;
            OnBallShot?.Invoke();
        }
        public void SetCanShootState(bool pCanShoot)
        {
            _canShoot = pCanShoot;
        }
        public void OnTimerEnded()
        {

        }
    }
}