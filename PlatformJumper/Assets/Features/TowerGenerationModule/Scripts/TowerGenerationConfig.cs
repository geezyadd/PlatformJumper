using UnityEngine;

namespace Features.TowerGenerationModule.Scripts {
    [CreateAssetMenu(fileName = "TowerGenerationConfig", menuName = "TowerGeneration/Config")]
    public class TowerGenerationConfig : ScriptableObject {
        [SerializeField] private float _segmentHeight = 20.82f;
        [SerializeField] private float _platformOrbitRadius = 6.2f;
        [SerializeField] private int _poolPreloadCount = 5;
        [SerializeField] private float _minDeltaY = 1.2f;
        [SerializeField] private float _maxDeltaY = 2.5f;
        [SerializeField] private float _maxDeltaAngleDegrees = 45f;
        [SerializeField] private float _firstPlatformHeight = 1.5f;
        [SerializeField] [Range(0f, 1f)] private float _wallSpawnChance = 0.4f;
        [SerializeField] private CylinderSegmentView[] _cylinderSegmentPrefabs;
        [SerializeField] private GameObject[] _platformPrefabs;

        public float SegmentHeight => _segmentHeight;
        public float PlatformOrbitRadius => _platformOrbitRadius;
        public int PoolPreloadCount => _poolPreloadCount;
        public float MinDeltaY => _minDeltaY;
        public float MaxDeltaY => _maxDeltaY;
        public float MaxDeltaAngleDegrees => _maxDeltaAngleDegrees;
        public float FirstPlatformHeight => _firstPlatformHeight;
        public float WallSpawnChance => _wallSpawnChance;
        public CylinderSegmentView[] CylinderSegmentPrefabs => _cylinderSegmentPrefabs;
        public GameObject[] PlatformPrefabs => _platformPrefabs;

        public GameObject GetRandomPlatformPrefab(PlatformKind kind) {
            if (_platformPrefabs == null || _platformPrefabs.Length == 0)
                return null;

            int validCount = 0;
            for (int i = 0; i < _platformPrefabs.Length; i++) {
                if (MatchesKind(_platformPrefabs[i], kind))
                    validCount++;
            }

            if (validCount == 0)
                return null;

            int pick = Random.Range(0, validCount);
            for (int i = 0; i < _platformPrefabs.Length; i++) {
                GameObject prefab = _platformPrefabs[i];
                if (!MatchesKind(prefab, kind))
                    continue;
                if (pick == 0)
                    return prefab;
                pick--;
            }

            return null;
        }

        private static bool MatchesKind(GameObject prefab, PlatformKind kind) {
            if (prefab == null)
                return false;

            PlatformDefinition definition = prefab.GetComponent<PlatformDefinition>();
            PlatformKind resolved = definition != null ? definition.Kind : PlatformKind.Ledge;
            return resolved == kind;
        }
    }
}
