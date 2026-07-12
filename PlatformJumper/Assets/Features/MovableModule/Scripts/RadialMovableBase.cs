using Features.FloatingControllerModule;
using UnityEngine;

namespace Features.MovableModule.Scripts {
    public abstract class RadialMovableBase : MonoBehaviour {
        [SerializeField] private FloatingController _floatingController;
        [SerializeField] private Transform _orbitCenter;
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _radiusCorrectionStrength = 50f;
        [SerializeField] private float _rotationSpeed = 360f;
        [SerializeField] private Rigidbody _rb;

        private float _orbitRadius;

        public bool IsGrounded => _floatingController.RayDidHit;

        protected virtual void Awake() {
            CacheOrbitRadius();
        }

        protected virtual void FixedUpdate() {
            ConstrainOrbitRadius();
        }

        protected virtual void Update() {
            FaceTowardCenter();
        }

        private void CacheOrbitRadius() {
            if (_orbitCenter == null || _rb == null)
                return;

            Vector3 offset = _rb.position - _orbitCenter.position;
            offset.y = 0f;
            _orbitRadius = offset.magnitude;
        }

        public virtual void Move(float direction) {
            if (_rb == null || _orbitCenter == null)
                return;

            Vector3 offset = _rb.position - _orbitCenter.position;
            offset.y = 0f;

            if (offset.sqrMagnitude < 0.0001f)
                return;

            Vector3 tangent = Vector3.Cross(Vector3.up, offset).normalized;
            Vector3 velocity = _rb.linearVelocity;
            float verticalSpeed = velocity.y;

            float radialSpeed = Vector3.Dot(velocity, offset.normalized);
            velocity -= offset.normalized * radialSpeed;

            float tangentSpeed = Vector3.Dot(velocity, tangent);
            velocity -= tangent * tangentSpeed;
            velocity += tangent * (direction * _speed);
            velocity.y = verticalSpeed;

            _rb.linearVelocity = velocity;
            ConstrainOrbitRadius();
        }

        private void ConstrainOrbitRadius() {
            if (_rb == null || _orbitCenter == null || _orbitRadius <= 0f)
                return;

            Vector3 horizontal = _rb.position - _orbitCenter.position;
            horizontal.y = 0f;

            float currentRadius = horizontal.magnitude;
            if (currentRadius < 0.0001f)
                return;

            Vector3 radialDir = horizontal / currentRadius;

            Vector3 velocity = _rb.linearVelocity;
            float radialSpeed = Vector3.Dot(velocity, radialDir);
            _rb.linearVelocity = velocity - radialDir * radialSpeed;

            float radiusError = _orbitRadius - currentRadius;
            _rb.AddForce(radialDir * (radiusError * _radiusCorrectionStrength), ForceMode.Force);
        }

        private void FaceTowardCenter() {
            if (_orbitCenter == null)
                return;

            Vector3 toCenter = _orbitCenter.position - transform.position;
            toCenter.y = 0f;

            if (toCenter.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(toCenter.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime);
        }

        public virtual void Jump() {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(_rb.transform.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
