using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    public class TubeInstance : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _holeCircle;
        private MaterialPropertyBlock _propertyBlock;
        private bool _isBad;
        private int _shaderId;
        public bool IsBad => _isBad;

        private void Start()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _shaderId = Shader.PropertyToID("_ColorValue");
            CallColorEasing(false);
        }
        public void FlipHoleState()
        {
            Action<float> OnUpdate = (float v) =>
            {
                _propertyBlock.SetFloat(_shaderId, v);
                _holeCircle.SetPropertyBlock(_propertyBlock);
            };
            GameManager.Instance.EasingController.StartEase(_isBad ? 1 : 0, _isBad ? 0 : 1, 1, OnUpdate);
            _isBad = !_isBad;
        }
        public void RandomizeHoleState()
        {
            StartCoroutine(PrepareColorChange(2));
        }
        private void CallColorEasing(float pDefaultValue, bool pAnimate = true)
        {
            _isBad = UnityEngine.Random.Range(0, 2) == 1 ? false : true;
            if (!pAnimate)
            {
                _propertyBlock.SetFloat(_shaderId, _isBad ? 0 : 1);
                _holeCircle.SetPropertyBlock(_propertyBlock);
                return;
            }
            //if (_isBad == newState) return;

            Action<float> OnUpdate = (float v) =>
            {
                _propertyBlock.SetFloat(_shaderId, v);
                _holeCircle.SetPropertyBlock(_propertyBlock);
            };
            GameManager.Instance.EasingController.StartEase(_isBad ? 0 : 1, pDefaultValue, .5f, OnUpdate);
        }
        private void CallColorEasing(bool pAnimate = true)
        {
            CallColorEasing(_isBad ? 0 : 1);
            return;
            bool newState = UnityEngine.Random.Range(0, 2) == 1 ? false : true;
            //if (_isBad == newState) return;
            _isBad = newState;
            if (!pAnimate) return;
            Action<float> OnUpdate = (float v) =>
            {
                _propertyBlock.SetFloat(_shaderId, v);
                _holeCircle.SetPropertyBlock(_propertyBlock);
            };
            GameManager.Instance.EasingController.StartEase(_isBad ? 0 : 1, _isBad ? 1 : 0, 2, OnUpdate);
        }
        private IEnumerator PrepareColorChange(float pDelay)
        {
            float timePassed = 0;
            int startVal = _isBad ? 0 : 1;
            float val = 0;
            while (timePassed < pDelay)
            {
                timePassed += Time.deltaTime;
                val = (Mathf.Sin(timePassed * 6 + startVal) + 1) / 2;
                Debug.Log(val);
                _propertyBlock.SetFloat(_shaderId, val);
                _holeCircle.SetPropertyBlock(_propertyBlock);
                yield return 0;

            }
            CallColorEasing(val);
        }
    }
}
