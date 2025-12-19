using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    public class ThrowingHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody _ballRB;
        [SerializeField] private Transform _throwPos;
        [SerializeField] private float _maxThrowPower = 5;
        [SerializeField] private float _shootCooldown = 2;
        [SerializeField] private int _verticalSensetivity = 250;
        [SerializeField] private int _horizontalSensetivity = 250;
        private TrajectoryDrawer _drawer;

        private Vector3 _initalAimPos;
        private bool _isAiming;
        GameManager _manager => GameManager.Instance;

        private void Start()
        {
            _drawer = FindObjectOfType<TrajectoryDrawer>();
            GameManager.Instance.OnBallCompleted.AddListener((_) => RespawnBall());
        }
        private void Update()
        {
            if (!_manager.CanShoot) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            PhoneShootControlls();
#else
            PCShootControlls();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void PhoneShootControlls()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    _initalAimPos = touch.position;
                    _isAiming = true;
                    RespawnBall();
                }
                if (_isAiming)
                {
                    float finalThrowPower;
                    Vector3 throwVector;
                    CalculateThrow(out finalThrowPower, out throwVector);

                    if (touch.phase == TouchPhase.Ended)
                    {
                        ShootBall(throwVector, finalThrowPower);
                    }
                }
            }
        }
#else
        private void PCShootControlls()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _initalAimPos = Input.mousePosition;
                _isAiming = true;
                RespawnBall();
            }
            if (_isAiming)
            {
                float finalThrowPower;
                Vector3 throwVector;
                CalculateThrow(out finalThrowPower, out throwVector);

                if (Input.GetMouseButtonUp(0))
                {
                    ShootBall(throwVector, finalThrowPower);
                }
            }
        }
        /// <summary>
        /// Calculates resulting throw power and throw direction
        /// </summary>
        /// <param name="pFinalThrowPower"></param>
        /// <param name="pThrowVector"></param>
#endif
        private void CalculateThrow(out float pFinalThrowPower, out Vector3 pThrowVector)
        {
            Vector3 currPos = Input.mousePosition;
            float yOffset = _initalAimPos.y - currPos.y;
            yOffset = yOffset < 0 ? 0 : yOffset;

            float tensionRatio = (yOffset) / (_verticalSensetivity * 10);
            float horizontalRatio = (_initalAimPos.x - currPos.x) / _horizontalSensetivity;
            pFinalThrowPower = _maxThrowPower * tensionRatio;
            pThrowVector = Vector3.forward;
            pThrowVector.y = 1f * tensionRatio;
            Debug.Log(tensionRatio);
            pThrowVector = Quaternion.AngleAxis(horizontalRatio, Vector3.up) * pThrowVector;
            _drawer.DrawTrajectory(pFinalThrowPower, _ballRB.mass, pThrowVector);
        }

        private void ShootBall(Vector3 pThrowVector, float pFinalThrowPower)
        {
            _ballRB.isKinematic = false;
            _isAiming = false;
            Debug.Log("firing");
            _ballRB.position = _throwPos.position;
            _ballRB.velocity = Vector3.zero;
            _ballRB.AddForce(pThrowVector.normalized * pFinalThrowPower, ForceMode.Impulse);
            _drawer.HideTrajectory();
            _manager.CallBallShot();
            StartCoroutine(DoShootCooldown());
        }
        private void RespawnBall()
        {
            _ballRB.isKinematic = true;
            _ballRB.position = _throwPos.position;
            StopCoroutine(DoShootCooldown());
        }
        private IEnumerator DoShootCooldown()
        {
            yield return new WaitForSeconds(_shootCooldown);
            _manager.SetCanShootState(true);
        }
    }
}