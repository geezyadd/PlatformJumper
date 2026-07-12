using Features.InputModule.Realization.Scripts.Generated;
using UnityEngine;
using Zenject;

namespace Features.MovableModule.Scripts {
    public class SimpleRadialMovable : RadialMovableBase {
        private const float JumpInputThreshold = 0.7f;

        [Header("Wall Jump")]
        [SerializeField] private Transform _wallCheckOrigin;
        [SerializeField] private float _wallCheckDistance = 0.5f;
        [SerializeField] private LayerMask _wallMask = ~0;

        private IInputService _inputService;
        private int _cachedDirectionX;
        private bool _jumpHeld;
        private bool _jumpConsumed;

        private bool _canWallJump;
        private Collider _detectedWall;
        private Collider _usedWall;

        public bool CanWallJump => _canWallJump;

        [Inject]
        private void InjectDependencies(IInputService inputService) {
            _inputService = inputService;
        }

        private void OnEnable() {
            _inputService.Movement.VectorChangedPerformed += CacheInput;
            _inputService.Movement.VectorChangedCanceled += CacheInput;
        }

        private void OnDisable() {
            _inputService.Movement.VectorChangedPerformed -= CacheInput;
            _inputService.Movement.VectorChangedCanceled -= CacheInput;
        }

        private void CacheInput(Vector2 input) {
            if (input.x > 0)
                _cachedDirectionX = -1;
            else if (input.x < 0)
                _cachedDirectionX = 1;
            else
                _cachedDirectionX = 0;

            bool jumpHeld = input.y > JumpInputThreshold;
            _jumpHeld = jumpHeld;
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            Move(_cachedDirectionX);
            UpdateWallJumpState();

            if (!IsGrounded && !_canWallJump) {
                _jumpConsumed = false;
                return;
            }

            if (_jumpHeld && !_jumpConsumed) {
                if (!IsGrounded && _canWallJump)
                    _usedWall = _detectedWall;

                Jump();
                _jumpConsumed = true;
            }
        }

        private void UpdateWallJumpState() {
            if (IsGrounded || _wallCheckOrigin == null) {
                _canWallJump = false;
                _detectedWall = null;
                _usedWall = null;
                return;
            }

            _detectedWall = DetectWall();
            _canWallJump = _detectedWall != null && _detectedWall != _usedWall;
        }

        private Collider DetectWall() {
            Vector3 origin = _wallCheckOrigin.position;
            Vector3 right = transform.right;

            bool hitLeft = Physics.Raycast(
                origin,
                -right,
                out RaycastHit leftHit,
                _wallCheckDistance,
                _wallMask,
                QueryTriggerInteraction.Ignore);
            bool hitRight = Physics.Raycast(
                origin,
                right,
                out RaycastHit rightHit,
                _wallCheckDistance,
                _wallMask,
                QueryTriggerInteraction.Ignore);

            if (hitLeft && leftHit.collider != _usedWall)
                return leftHit.collider;
            if (hitRight && rightHit.collider != _usedWall)
                return rightHit.collider;
            if (hitLeft)
                return leftHit.collider;
            if (hitRight)
                return rightHit.collider;

            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_wallCheckOrigin == null)
                return;

            Vector3 origin = _wallCheckOrigin.position;
            Vector3 right = transform.right;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + right * _wallCheckDistance);
            Gizmos.DrawLine(origin, origin - right * _wallCheckDistance);
        }
#endif
    }
}
