using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallThrowGame
{
    public class ThrowingHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody _ballRB;
        [SerializeField] private Transform _throwPos;
        [SerializeField] private float _throwPower = 5;

        private bool _canThrow;
        private TrajectoryDrawer _drawer;

        private Vector3 _initalAimPos;
        private bool _isAiming;
        private void Start()
        {
            _drawer = FindObjectOfType<TrajectoryDrawer>();
            GameManager.Instance.OnBallCompleted.AddListener((_) => RespawnBall());
        }
        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    _initalAimPos = touch.position;
                    _isAiming = true;
                }
                if (_isAiming)
                {
                    Vector3 currPos = touch.position;
                    float yOffset = _initalAimPos.y - currPos.y;
                    yOffset = yOffset < 0 ? 0 : yOffset;

                    float tensionRatio = (yOffset) / 200f;
                    float horizontalRatio = _initalAimPos.x - currPos.x;
                    float finalThrowPower = _throwPower * tensionRatio;

                    Vector3 throwVector = Vector3.forward;
                    throwVector.y = 1f * tensionRatio;
                    throwVector = Quaternion.AngleAxis(horizontalRatio, Vector3.up) * throwVector;
                    _drawer.DrawTrajectory(finalThrowPower, _ballRB.mass, throwVector);
                    if (touch.phase == TouchPhase.Ended)
                    {
                        ShootBall(throwVector, finalThrowPower);
                    }
                }
            }
            return;
            if (Input.GetMouseButtonDown(0))
            {
                _initalAimPos = Input.mousePosition;
                _isAiming = true;
            }
            if (_isAiming)
            {
                Vector3 currPos = Input.mousePosition;
                float yOffset = _initalAimPos.y - currPos.y;
                yOffset = yOffset < 0 ? 0 : yOffset;

                float tensionRatio = (yOffset) / 200f;
                float horizontalRatio = _initalAimPos.x - currPos.x;
                float finalThrowPower = _throwPower * tensionRatio;

                Vector3 throwVector = Vector3.forward;
                throwVector.y = 1f * tensionRatio;
                throwVector = Quaternion.AngleAxis(horizontalRatio, Vector3.up) * throwVector;
                _drawer.DrawTrajectory(finalThrowPower, _ballRB.mass, throwVector);
                if (Input.GetMouseButtonUp(0))
                {
                    ShootBall(throwVector, finalThrowPower);
                }
            }
        }
        private void ShootBall(Vector3 pThrowVector, float pFinalThrowPower)
        {
            _isAiming = false;
            Debug.Log("firing");
            _ballRB.position = _throwPos.position;
            _ballRB.velocity = Vector3.zero;
            _ballRB.AddForce(pThrowVector.normalized * pFinalThrowPower, ForceMode.Impulse);
        }
        private void RespawnBall()
        {
            _ballRB.isKinematic = true;
            _ballRB.position = _throwPos.position;
        }
    }
}