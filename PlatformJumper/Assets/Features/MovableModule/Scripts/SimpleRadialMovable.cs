using Features.InputModule.Realization.Scripts.Generated;
using UnityEngine;
using Zenject;

namespace Features.MovableModule.Scripts {
    public class SimpleRadialMovable : RadialMovableBase {
        private const float JumpInputThreshold = 0.7f;

        private IInputService _inputService;
        private int _cachedDirectionX;
        private bool _jumpHeld;
        private bool _jumpConsumed;

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

        protected override void Update() {
            base.Update();
            Move(_cachedDirectionX);

            if (!IsGrounded) {
                _jumpConsumed = false;
                return;
            }

            if (_jumpHeld && !_jumpConsumed) {
                Jump();
                _jumpConsumed = true;
            }
        }
    }
}
