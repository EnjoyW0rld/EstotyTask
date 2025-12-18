using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BallThrowGame
{
    public class GameManager : MonoBehaviour
    {
        [Header("Holes variables")]
        [SerializeField] private int _holePositionChangeCooldown;
        private EasingController _easingController;
        //Events
        public UnityEvent<CompletionType> OnBallCompleted;
        // Enums
        public enum CompletionType { Bad, Neutral, Good }
        //Singlton variables
        public int HolePositionChangeCooldown => _holePositionChangeCooldown;
        public EasingController EasingController => _easingController;

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
        }
        public void CallBallComplete(CompletionType pType)
        {
            OnBallCompleted?.Invoke(pType);
        }
    }
}