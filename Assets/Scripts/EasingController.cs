using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BallThrowGame
{

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
                    //float currValue = (progress);
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
            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static float SineEaseOut(float t, float b, float c, float d)
            {
                return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
            }
            public static float SineEaseToCubic(float t)
            {
                return Mathf.Lerp((Mathf.Sin(t * 5) + 1) / 2, t, t);
                if (t > .5f)
                {
                    return EaseInCubic(t);
                }
                else
                {
                    return (Mathf.Sin(t) + 1) / 2;
                }
            }

            private float easeInOutCirc(float x)
            {
                return x < 0.5
                  ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
                  : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
            }
        }
    }
}