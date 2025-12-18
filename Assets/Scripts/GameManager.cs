using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    public class GameManager : MonoBehaviour
    {
        [Header("Holes variables")]
        [SerializeField] private int _holePositionChangeCooldown;
        public int HolePositionChangeCooldown { get;private set; }


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
        }
    }
}