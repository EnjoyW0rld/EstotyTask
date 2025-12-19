using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BallThrowGame
{
    //*************************************************************************
    // Script to be applied on the single ball instance.
    // Checks collisions with triggers and calls corresponding function in GameManager
    //*************************************************************************

    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private string _holeEntranceTag;
        [SerializeField] private string _holeExitTag;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField, Tooltip("Amount of velocity to consider impact strong enough to play sound")]
        private float _velocityThreshold;
        public UnityEvent OnHit;

        private GameManager manager => GameManager.Instance;
        private Rigidbody _rb;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            if(OnHit == null) OnHit = new UnityEvent();
        }
        private void Update()
        {
            if (_rb.position.y < manager.YDeathZone)
            {
                manager.CallBallComplete(GameManager.CompletionType.Neutral);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_holeEntranceTag))
            {
                _rb.excludeLayers |= _groundLayer;
            }
            else if (other.CompareTag(_holeExitTag))
            {
                TubeInstance completedTube = other.GetComponentInParent<TubeInstance>();
                if (!completedTube.IsBad) completedTube.FireParticles();
                GameManager.Instance.CallBallComplete(completedTube.IsBad ? GameManager.CompletionType.Bad : GameManager.CompletionType.Good);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_holeEntranceTag))
            {
                _rb.excludeLayers &= ~_groundLayer;
            }

        }
        private void OnCollisionEnter(Collision collision)
        {
            if (_rb.velocity.magnitude > _velocityThreshold) OnHit?.Invoke();
        }
    }
}