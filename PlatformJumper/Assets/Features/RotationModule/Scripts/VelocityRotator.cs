using UnityEngine;

namespace Features.RotationModule.Scripts {
    public class VelocityRotator : MonoBehaviour {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _target;
        [SerializeField] private bool _zeroY;
        [SerializeField] private float _rotationSpeed = 360f;
        [SerializeField] private float _minSpeed = 0.01f;

        private void Update() {
            if (_rigidbody == null || _target == null)
                return;

            Vector3 velocity = _rigidbody.linearVelocity;
            if (_zeroY)
                velocity.y = 0f;

            if (velocity.sqrMagnitude < _minSpeed * _minSpeed)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            _target.rotation = Quaternion.RotateTowards(
                _target.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime);
        }
    }
}
