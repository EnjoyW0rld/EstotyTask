using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BallThrowGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private string _holeEntranceTag;
        [SerializeField] private string _holeExitTag;
        [SerializeField] private LayerMask _groundLayer;
        private Rigidbody _rb;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_holeEntranceTag))
            {
                _rb.excludeLayers |= _groundLayer;
                Debug.Log("excluding");
            }
            else if (other.CompareTag(_holeExitTag))
            {
                TubeInstance completedTube = other.GetComponentInParent<TubeInstance>();
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
    }
}