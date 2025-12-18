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
        private TubeInstance[] _instancedTubes;
        private int _laneCount;
        private void Start()
        {
            _laneCount = (int)((_placingArea.height / 2) / _tubeRadius);
            _instancedTubes = new TubeInstance[_laneCount];

            float ySide = _placingArea.position.y - _placingArea.height/2;
            for (int i = 0; i < _laneCount; i++)
            {
                _instancedTubes[i] = Instantiate(_tubePrefab);
                _instancedTubes[i].transform.position = new Vector3(0, 0, ySide + (_tubeRadius * 2) * i + _tubeRadius);
            }
        }
        private void RefreshHolesPosition()
        {
            float xSide = _placingArea.center.x - _placingArea.width/2;
            float maxPlacingRange = _placingArea.width / 2 - _tubeRadius;
            for (int i = 0; i < _laneCount; i++)
            {
                Vector3 currentPos = _instancedTubes[i].transform.position;
                currentPos.x = Random.Range(-maxPlacingRange, maxPlacingRange) + xSide;
                _instancedTubes[i].transform.position = currentPos;
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                RefreshHolesPosition();
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