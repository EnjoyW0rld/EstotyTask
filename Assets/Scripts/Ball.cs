using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private string _holeEntranceTag;
        [SerializeField] private LayerMask _groundLayer;
        private Rigidbody _rb;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            Debug.Log($"{_rb.excludeLayers.value}");

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_holeEntranceTag))
            {
                _rb.excludeLayers |= _groundLayer;
                Debug.Log("excluding");
                //_rb.excludeLayers
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