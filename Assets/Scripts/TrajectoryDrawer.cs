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
        [SerializeField] private float _throwStrength = 10f;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _timeBetweenCalculations = .25f;
        [SerializeField] private int _linePoints = 25;


        public void DrawTrajectory(float pThrowStrength, float pMass, Vector3 pDir)
        {
            _targetTransform.gameObject.SetActive(true);
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenCalculations) + 1;
            Vector3 startPos = _throwPosition.position;
            Vector3 startVelocity = pThrowStrength * pDir.normalized / pMass;
            int i = 0;

            for (float time = 0; time < _linePoints; time += _timeBetweenCalculations)
            {
                Vector3 point = startPos + time * startVelocity;
                point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

                if (point.y < 0)
                {
                    point = CalculateLastPoint(i, point);
                    return;
                }
                _lineRenderer.SetPosition(i, point);
                i++;
            }

        }
        private Vector3 CalculateLastPoint(int pI, Vector3 pPoint)
        {
            Plane p = new Plane(Vector3.up, Vector3.right);
            Vector3 lastPos = _lineRenderer.GetPosition(pI - 1);
            Ray rayFromLastPos = new Ray(lastPos, (pPoint - lastPos).normalized);
            p.Raycast(rayFromLastPos, out float enter);
            pPoint = rayFromLastPos.GetPoint(enter);

            _lineRenderer.SetPosition(pI, pPoint);
            pPoint.y += .01f;
            _targetTransform.position = pPoint;
            _lineRenderer.positionCount = pI + 1;
            return pPoint;
        }

        public void HideTrajectory()
        {
            _lineRenderer.enabled = false;
            _targetTransform.gameObject.SetActive(false);
        }

    }
}