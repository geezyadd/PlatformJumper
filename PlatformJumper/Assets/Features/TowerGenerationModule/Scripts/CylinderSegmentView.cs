using UnityEngine;

namespace Features.TowerGenerationModule.Scripts {
    public class CylinderSegmentView : MonoBehaviour {
        [SerializeField] private Transform _platformsRoot;

        public int Index { get; private set; }
        public int PrefabIndex { get; set; }
        public Transform PlatformsRoot => _platformsRoot;
        public float LastPlatformAngle { get; set; }
        public float LastPlatformWorldY { get; set; }

        public void OnSpawned(int index, Vector3 worldPosition) {
            Index = index;
            transform.position = worldPosition;
            gameObject.SetActive(true);
        }

        public void OnDespawned() {
            ClearPlatforms();
            gameObject.SetActive(false);
        }

        public void ClearPlatforms() {
            if (_platformsRoot == null)
                return;

            for (int i = _platformsRoot.childCount - 1; i >= 0; i--)
                Destroy(_platformsRoot.GetChild(i).gameObject);
        }
    }
}
