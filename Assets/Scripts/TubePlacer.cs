using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    public class TubePlacer : MonoBehaviour
    {
        [SerializeField] private TubeInstance _tubePrefab;
        [SerializeField] private Rect _placingArea;
        [SerializeField] private float _tubeRadius = 2;
        [SerializeField] private float _fadeDuration = 1;
        [Header("Cooldowns")]
        [SerializeField] private float _positionChangeCooldown = 10;
        [SerializeField] private float _stateChangeMinCooldown = 4;
        [SerializeField] private float _stateChangeMaxCooldown = 7;
        private bool _canChangePosition;

        private TubeInstance[] _instancedTubes;
        private int _laneCount;

        private Action _randomizeHolePosition;
        private Action _randomizeHoleState;
        private void Start()
        {
            _laneCount = (int)((_placingArea.height / 2) / _tubeRadius);
            _instancedTubes = new TubeInstance[_laneCount];
            float ySide = _placingArea.position.y - _placingArea.height / 2;
            for (int i = 0; i < _laneCount; i++)
            {
                _instancedTubes[i] = Instantiate(_tubePrefab);
                _instancedTubes[i].transform.position = new Vector3(0, 0, ySide + (_tubeRadius * 2) * i + _tubeRadius);

                InitHolePositionAction(i);
                InitHoleStateAction(i);
            }
            StartCoroutine(DoColorChangeCooldown());

        }
        private void InitHoleStateAction(int i)
        {
            _randomizeHoleState += () =>
            {
                _instancedTubes[i].RandomizeHoleState();
            };
        }
        private void InitHolePositionAction(int i)
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
                    RefreshHolesPosition();
                    GameManager.Instance.EasingController.StartEase(1, 0, _fadeDuration, (float v) =>
                    {
                        _instancedTubes[num].transform.localScale = new Vector3(v, 1, v);
                    });
                };
                GameManager.Instance.EasingController.StartEase(0, 1, _fadeDuration, OnUpdate, OnComplete);
            };
        }
        private void RefreshHolesPosition()
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
        private void RandomizeHoleState()
        {
            _randomizeHoleState?.Invoke();
            StartCoroutine(DoColorChangeCooldown());
        }
        private IEnumerator DoPositionChangeCooldown()
        {
            yield return new WaitForSeconds(_positionChangeCooldown);
            _canChangePosition = true;
        }
        private IEnumerator DoColorChangeCooldown()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_stateChangeMinCooldown,_stateChangeMaxCooldown));
            RandomizeHoleState();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _randomizeHolePosition?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                EasingController c = FindObjectOfType<EasingController>();
                for (int i = 0; i < _instancedTubes.Length; i++)
                {
                    int num = i;
                    c.StartEase(0, 1, 2, (float v) =>
                    {
                        _instancedTubes[num].transform.localScale = new Vector3(v, 1, v);
                        Debug.Log(v);
                    });
                }
            }
        }
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