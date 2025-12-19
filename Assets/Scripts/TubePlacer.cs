using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    //************************************************************************************
    // Handles tube placement, position randomization and color change.
    // Keeps track of the corresponding cooldowns
    //************************************************************************************
    public class TubePlacer : MonoBehaviour
    {
        [SerializeField] private TubeInstance _tubePrefab;
        [SerializeField] private Rect _placingArea;
        [SerializeField, Tooltip("Check context menu for set up")] 
        private float _tubeRadius = 2;
        [Header("Cooldowns")]
        [SerializeField, Tooltip("Cooldown before tube position randomization")]
        private float _positionChangeCooldown = 10;
        [SerializeField] private float _stateChangeMinCooldown = 4;
        [SerializeField] private float _stateChangeMaxCooldown = 7;
        [Header("Animations")]
        [SerializeField] private float _fadeDuration = 1;
        private bool _needChangePosition;

        private TubeInstance[] _instancedTubes;
        private int _laneCount;

        private Action _randomizeHolePosition;
        private Action _randomizeHoleState;

        private void Start()
        {
            // Instantiating all tubes and setting up actions to call position/state change
            _laneCount = (int)((_placingArea.height / 2) / _tubeRadius);
            _instancedTubes = new TubeInstance[_laneCount];
            float ySide = _placingArea.position.y - _placingArea.height / 2;
            for (int i = 0; i < _laneCount; i++)
            {
                _instancedTubes[i] = Instantiate(_tubePrefab);
                _instancedTubes[i].transform.position = new Vector3(0, 0, ySide + (_tubeRadius * 2) * i + _tubeRadius);

                InitHolePositionAction(i, i == 0);
                InitHoleStateAction(i);
            }
            GameManager.Instance.OnBallCompleted.AddListener((_) => TrySwitchTubePlaces());
            GameManager.Instance.OnGameStarted.AddListener(InitiateTubePlacement);
            GameManager.Instance.OnTimerElapsed.AddListener(StopTubePlacement);
        }
        private void InitiateTubePlacement()
        {
            // Start cooldown coroutines
            StartCoroutine(DoPositionChangeCooldown());
            StartCoroutine(DoColorChangeCooldown());
            CalculateNewHolePos();
        }
        private void StopTubePlacement()
        {
            StopCoroutine(DoPositionChangeCooldown());
            StopCoroutine(DoColorChangeCooldown());
        }
        // Initializers
        private void InitHoleStateAction(int i)
        {
            _randomizeHoleState += () =>
            {
                _instancedTubes[i].RandomizeHoleState();
            };
        }
        private void InitHolePositionAction(int i, bool pCallHandlerFunctions = false)
        {
            _randomizeHolePosition += () =>
            {
                int num = i;
                // First we call an ease out to make holes dissapear
                Action<float> OnUpdate = (float v) =>
                {
                    _instancedTubes[num].transform.localScale = new Vector3(v, 1, v);
                };
                // On ease out finish we calculate new holes position and call holes ease in
                Action OnComplete = () =>
                {
                    if (pCallHandlerFunctions)
                    {
                        CalculateNewHolePos();
                        GameManager.Instance.EasingController.StartEase(1, 0, _fadeDuration, (float v) =>
                        {
                            _instancedTubes[num].transform.localScale = new Vector3(v, 1, v);
                        }, OnHolePositionsUpdated);
                    }
                    else
                    {
                        GameManager.Instance.EasingController.StartEase(1, 0, _fadeDuration, (float v) =>
                        {
                            _instancedTubes[num].transform.localScale = new Vector3(v, 1, v);
                        });
                    }
                };
                GameManager.Instance.EasingController.StartEase(0, 1, _fadeDuration, OnUpdate, OnComplete);
            };
        }
        // Position change functions
        private void OnHolePositionsUpdated()
        {
            GameManager.Instance.SetCanShootState(true);
            StartCoroutine(DoPositionChangeCooldown());
        }
        private void CalculateNewHolePos()
        {
            float xSide = _placingArea.center.x - _placingArea.width / 2;
            float maxPlacingRange = _placingArea.width / 2 - _tubeRadius;
            for (int i = 0; i < _laneCount; i++)
            {
                Vector3 currentPos = _instancedTubes[i].transform.position;
                currentPos.x = UnityEngine.Random.Range(-maxPlacingRange, maxPlacingRange) + xSide;
                _instancedTubes[i].transform.position = currentPos;
            }

        }
        private void TrySwitchTubePlaces()
        {
            if (_needChangePosition)
            {
                GameManager.Instance.SetCanShootState(false);
                _randomizeHolePosition?.Invoke();
                _needChangePosition = false;
            }
        }
        private IEnumerator DoPositionChangeCooldown()
        {
            yield return new WaitForSeconds(_positionChangeCooldown + _fadeDuration * 2);
            _needChangePosition = true;
        }
        // State change functions
        private void RandomizeHoleState()
        {
            _randomizeHoleState?.Invoke();
            StartCoroutine(DoColorChangeCooldown());
        }
        private IEnumerator DoColorChangeCooldown()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_stateChangeMinCooldown, _stateChangeMaxCooldown));
            RandomizeHoleState();
        }

        // Editor functions to make radius set up easier
        #region FUNCTIONS_FOR_EDITOR
#if UNITY_EDITOR
        private GameObject _tempObject;
        [ContextMenu("Start radius set up")]
        private void StartSetUp()
        {
            if (_tubePrefab == null)
            {
                Debug.LogAssertion("Cannot start set up because prefab is not set");
                return;
            }
            _tempObject = Instantiate(_tubePrefab).gameObject;

        }
        [ContextMenu("Finish radius set up")]
        private void FinishSetUp()
        {
            DestroyImmediate(_tempObject);
        }
#endif
        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (_tempObject != null)
            {
                Gizmos.DrawWireSphere(_tempObject.transform.position, _tubeRadius);
            }
#endif
            //Vector3 centerPos = new Vector3(_placingArea.center.x - _placingArea.width / 2, 0, _placingArea.center.y - _placingArea.height / 2);
            Vector3 centerPos = new Vector3(_placingArea.position.x, 0, _placingArea.position.y);
            Vector3 size = new Vector3(_placingArea.width, 0, _placingArea.height);
            Gizmos.DrawWireCube(centerPos, size);
            Gizmos.DrawSphere(new Vector3(_placingArea.center.x, 0, _placingArea.center.y), 1);

        }
        #endregion
    }
}