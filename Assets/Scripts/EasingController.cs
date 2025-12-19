using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BallThrowGame
{
    //*****************************************************************************************************
    //  NOTE: Functionality is not implemented to the fullest due to being unnecesary in current assignment
    //        In future versions to add more easing functions and enum to choose which to use
    //*****************************************************************************************************

    public class EasingController : MonoBehaviour
    {
        private List<EaseSettings> settings;
        private void Start()
        {
            settings = new List<EaseSettings>();
        }
        private void Update()
        {
            if (settings.Count > 0)
            {
                for (int i = settings.Count - (1); i >= 0; i--)
                {
                    if (settings[i].DoEase(Time.deltaTime))
                    {
                        settings.RemoveAt(i);
                    }
                }

            }
        }
        public void StartEase(float pTargetValue, float pDefaultValue, float pDuration, Action<float> pOnUpdate, Action pOnComplete = null)
        {
            EaseSettings newEase = new EaseSettings();
            newEase.Initialize(pTargetValue, pDefaultValue, pDuration, pOnUpdate, pOnComplete);
            settings.Add(newEase);
        }
        private class EaseSettings
        {
            private float _targetValue;
            private float _defaultValue;
            private float _time;
            private float _duration;
            private Action<float> OnUpdate;
            private Action OnComplete;

            public void Initialize(float pTargetValue, float pDefaultValue, float pDuration, Action<float> pOnUpdate, Action pOnComplete = null)
            {
                _targetValue = pTargetValue;
                _defaultValue = pDefaultValue;
                _duration = pDuration;
                OnUpdate = pOnUpdate;
                OnComplete = pOnComplete;
                _time = 0;
            }
            public bool DoEase(float pDeltaTime)
            {
                _time = _time + pDeltaTime;
                float progress = _time / _duration;
                if (progress >= 1)
                {
                    OnUpdate?.Invoke(_targetValue);
                    OnComplete?.Invoke();
                    return true;
                }
                else
                {
                    float currValue = Mathf.Lerp(_defaultValue, _targetValue, EaseInCubic(progress));
                    OnUpdate.Invoke(currValue);
                    return false;
                }
            }
            //Easing functions
            public static float EaseInCubic(float t)
            {
                return t * t * t;
            }
        }
    }
}