using UnityEngine;

namespace Features.AnimationModule.Scripts {
    public abstract class AnimationControllerBase : MonoBehaviour {
        [SerializeField] private Animator _animator;

        protected Animator Animator => _animator;

        protected void SetTrigger(int id) {
            _animator.SetTrigger(id);
        }

        protected void SetTrigger(string name) {
            _animator.SetTrigger(name);
        }

        protected void ResetTrigger(int id) {
            _animator.ResetTrigger(id);
        }

        protected void ResetTrigger(string name) {
            _animator.ResetTrigger(name);
        }

        protected void SetBool(int id, bool value) {
            _animator.SetBool(id, value);
        }

        protected void SetBool(string name, bool value) {
            _animator.SetBool(name, value);
        }

        protected bool GetBool(int id) {
            return _animator.GetBool(id);
        }

        protected bool GetBool(string name) {
            return _animator.GetBool(name);
        }

        protected void SetFloat(int id, float value) {
            _animator.SetFloat(id, value);
        }

        protected void SetFloat(string name, float value) {
            _animator.SetFloat(name, value);
        }

        protected void SetFloat(int id, float value, float dampTime, float deltaTime) {
            _animator.SetFloat(id, value, dampTime, deltaTime);
        }

        protected void SetFloat(string name, float value, float dampTime, float deltaTime) {
            _animator.SetFloat(name, value, dampTime, deltaTime);
        }

        protected float GetFloat(int id) {
            return _animator.GetFloat(id);
        }

        protected float GetFloat(string name) {
            return _animator.GetFloat(name);
        }

        protected void SetInteger(int id, int value) {
            _animator.SetInteger(id, value);
        }

        protected void SetInteger(string name, int value) {
            _animator.SetInteger(name, value);
        }

        protected int GetInteger(int id) {
            return _animator.GetInteger(id);
        }

        protected int GetInteger(string name) {
            return _animator.GetInteger(name);
        }
    }
}
