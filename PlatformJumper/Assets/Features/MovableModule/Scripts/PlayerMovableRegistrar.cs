using UnityEngine;
using Zenject;

namespace Features.MovableModule.Scripts {
    public class PlayerMovableRegistrar : MonoBehaviour {
        [SerializeField] private RadialMovableBase _radialMovable;
        private MovableModel _movableModel;

        [Inject]
        private void InjectDependencies(MovableModel movableModel) {
            _movableModel = movableModel;
        }

        private void OnEnable() {
            _movableModel.PlayerMovable = _radialMovable;
        }
    }
}