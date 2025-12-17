using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
namespace BallThrowGame
{
    public class TrajectoryDrawer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _throwPosition;
        [SerializeField] private Vector3 dir = Vector3.right;
        [SerializeField] private float _throwStrength = 10f;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _timeBetweenCalculations = .25f;
        [SerializeField] private int _linePoints = 25;
        private float _relativeMinY;
        private void Start()
        {
            _relativeMinY = -transform.position.y;
        }
        /*public void DrawTrajectory()
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenCalculations) + 1;
            Vector3 startPos = _throwPosition.position;
            Vector3 startVelocity = _throwStrength * dir.normalized / _ballRB.mass;
            int i = 0;
            _lineRenderer.SetPosition(i, startPos);
            for (float time = 0; time < _linePoints; time += _timeBetweenCalculations)
            {
                i++;
                Vector3 point = startPos + time * startVelocity;
                point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                _lineRenderer.SetPosition(i, point);
            }
        }*/
        public void DrawTrajectory(float pThrowStrength, float pMass, Vector3 pDir)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenCalculations) + 1;
            Vector3 startPos = _throwPosition.position;
            Vector3 startVelocity = pThrowStrength * pDir.normalized / pMass;
            int i = 0;
            //_lineRenderer.SetPosition(i, startPos);
            for (float time = 0; time < _linePoints; time += _timeBetweenCalculations)
            {
                Vector3 point = startPos + time * startVelocity;
                point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                Vector3 pointInWorldSpace = transform.localToWorldMatrix * point;

                if (point.y < 0)
                {
                    Plane p = new Plane(Vector3.up, Vector3.right);
                    Vector3 lastPos = _lineRenderer.GetPosition(i - 1);
                    Ray rayFromLastPos = new Ray(lastPos, (point - lastPos).normalized);
                    p.Raycast(rayFromLastPos, out float enter);
                    point = rayFromLastPos.GetPoint(enter);

                    _lineRenderer.SetPosition(i, point);
                    point.y += .01f;
                    _targetTransform.position = point;
                    _lineRenderer.positionCount = i + 1;
                    return;
                }
                _lineRenderer.SetPosition(i, point);
                i++;
            }
        }
        public void HideTrajectory()
        {
            _lineRenderer.enabled = false;
        }

    }
}