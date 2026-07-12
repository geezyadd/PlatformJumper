using Features.MovableModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.AnimationModule.Scripts {
    public class PlayerAnimationController : AnimationControllerBase {
        [SerializeField] private float _smoothingSpeed = 4;
        private MovableModel _movableModel;

        [Inject]
        private void InjectDependencies(MovableModel movableModel) {
            _movableModel = movableModel;
        }

        private void Update() {
            SetBool("IsGrounded", _movableModel.PlayerMovable.IsGrounded);
            Vector3 velocity = _movableModel.PlayerMovable.Rigidbody.linearVelocity;
            velocity.y = 0;
            SetFloat("Velocity", Mathf.Lerp(GetFloat("Velocity"), velocity.magnitude,  Time.deltaTime * _smoothingSpeed));
        }
    }
}