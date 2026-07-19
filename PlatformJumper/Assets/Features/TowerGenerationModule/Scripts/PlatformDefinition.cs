using UnityEngine;

namespace Features.TowerGenerationModule.Scripts {
    public class PlatformDefinition : MonoBehaviour {
        [SerializeField] private PlatformKind _kind = PlatformKind.Ledge;

        public PlatformKind Kind => _kind;
    }
}
