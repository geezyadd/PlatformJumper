using System.Collections.Generic;
using UnityEngine;

namespace Features.FloatingControllerModule {
    public class FloatingController : MonoBehaviour {
        [Header("Cast")]
        [SerializeField] private Vector3 _downDir = Vector3.down;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _castLength = 3f;
        [SerializeField] private List<Transform> _castPoints = new();

        [Header("Ride Settings")]
        [SerializeField] private float _rideHeight = 1.5f;
        [SerializeField] private float _rideSpringStrength = 100f;
        [SerializeField] private float _rideSpringDamper = 10f;

        [Header("Force")]
        [SerializeField] private bool _applyForceToHitBody = true;
        [SerializeField] private Rigidbody _rb;

        private RaycastHit _hit;
        private bool _hasHit;

        public bool RayDidHit => _hasHit;

        private void FixedUpdate() {
            SampleHit();
            if (_hasHit)
                ApplySpringForce();
        }

        private void SampleHit() {
            _hasHit = false;
            Vector3 castDir = transform.TransformDirection(_downDir.normalized);

            for (int i = 0; i < _castPoints.Count; i++) {
                Transform castPoint = _castPoints[i];
                if (castPoint == null)
                    continue;

                if (!Physics.Raycast(
                        castPoint.position,
                        castDir,
                        out _hit,
                        _castLength,
                        _groundMask,
                        QueryTriggerInteraction.Ignore))
                    continue;

                _hasHit = true;
                return;
            }
        }

        private void ApplySpringForce() {
            if (_rb == null)
                return;

            Vector3 castDir = transform.TransformDirection(_downDir.normalized);
            float bodyDirVelocity = Vector3.Dot(castDir, _rb.linearVelocity);

            float hitBodyDirVelocity = 0f;
            Rigidbody hitBody = _hit.rigidbody;
            if (hitBody != null)
                hitBodyDirVelocity = Vector3.Dot(castDir, hitBody.linearVelocity);

            float relativeVelocity = bodyDirVelocity - hitBodyDirVelocity;
            float compression = _hit.distance - _rideHeight;
            float springForce = (compression * _rideSpringStrength) - (relativeVelocity * _rideSpringDamper);

            _rb.AddForce(castDir * springForce, ForceMode.Force);

            if (_applyForceToHitBody && hitBody != null)
                hitBody.AddForceAtPosition(castDir * -springForce, _hit.point, ForceMode.Force);
        }

        private void OnDrawGizmosSelected() {
            Vector3 castDir = transform.TransformDirection(_downDir.normalized);

            for (int i = 0; i < _castPoints.Count; i++) {
                Transform castPoint = _castPoints[i];
                if (castPoint == null)
                    continue;

                Vector3 origin = castPoint.position;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(origin, origin + castDir * _castLength);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(origin + castDir * _rideHeight, 0.05f);
            }
        }
    }
}
