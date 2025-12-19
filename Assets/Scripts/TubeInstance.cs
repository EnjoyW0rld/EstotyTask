using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BallThrowGame
{
    public class TubeInstance : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _holeCircle;
        [SerializeField] private ParticleSystem _particles;
        private MaterialPropertyBlock _propertyBlock;
        private bool _isBad;
        private int _colorParameterID;
        public bool IsBad => _isBad;

        private void Start()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _colorParameterID = Shader.PropertyToID("_ColorValue");
            CallColorEasing(false);
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
                _propertyBlock.SetFloat(_colorParameterID, _isBad ? 0 : 1);
                _holeCircle.SetPropertyBlock(_propertyBlock);
                return;
            }

            Action<float> OnUpdate = (float v) =>
            {
                _propertyBlock.SetFloat(_colorParameterID, v);
                _holeCircle.SetPropertyBlock(_propertyBlock);
            };
            GameManager.Instance.EasingController.StartEase(_isBad ? 0 : 1, pDefaultValue, .5f, OnUpdate);
        }
        private void CallColorEasing(bool pAnimate = true)
        {
            CallColorEasing(_isBad ? 0 : 1);
        }
        public void FireParticles()
        {
            _particles.Play();
        }
        /// <summary>
        /// Coroutine drawing blinking indication before actual color change
        /// </summary>
        /// <param name="pDelay">Blinking duration</param>
        /// <returns></returns>
        private IEnumerator PrepareColorChange(float pDelay)
        {
            float timePassed = 0;
            int startVal = _isBad ? 0 : 1;
            float val = 0;
            while (timePassed < pDelay)
            {
                timePassed += Time.deltaTime;
                val = (Mathf.Sin(timePassed * 6 + startVal) + 1) / 2;
                _propertyBlock.SetFloat(_colorParameterID, val);
                _holeCircle.SetPropertyBlock(_propertyBlock);
                yield return 0;

            }
            CallColorEasing(val);
        }
    }
}
