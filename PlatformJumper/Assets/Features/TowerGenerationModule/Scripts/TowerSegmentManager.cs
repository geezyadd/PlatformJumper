using System.Collections.Generic;
using Features.MovableModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.TowerGenerationModule.Scripts {
    public class TowerSegmentManager : MonoBehaviour {
        [SerializeField] private TowerGenerationConfig _config;
        [SerializeField] private Transform _orbitAxis;
        [SerializeField] private Transform _segmentsRoot;

        private readonly Dictionary<int, CylinderSegmentView> _activeSegments =
            new Dictionary<int, CylinderSegmentView>();
        private readonly List<int> _removeBuffer = new List<int>();

        private MovableModel _movableModel;
        private CylinderSegmentPool _pool;
        private PlatformGenerator _platformGenerator;
        private float _routeAngle;
        private float _routeWorldY;
        private int _highestGeneratedIndex = -1;

        [Inject]
        private void InjectDependencies(MovableModel movableModel) {
            _movableModel = movableModel;
        }

        private void Awake() {
            Transform parent = _segmentsRoot != null ? _segmentsRoot : transform;
            _pool = new CylinderSegmentPool(_config.CylinderSegmentPrefabs, parent);
            _pool.Preload(_config.PoolPreloadCount);
            _platformGenerator = new PlatformGenerator(_config);
        }

        private void Start() {
            SyncWindow();
        }

        private void Update() {
            SyncWindow();
        }

        private void SyncWindow() {
            if (_movableModel?.PlayerMovable == null || _config == null)
                return;

            float playerY = _movableModel.PlayerMovable.transform.position.y;
            int currentIndex = Mathf.FloorToInt(playerY / _config.SegmentHeight);
            if (currentIndex < 0)
                currentIndex = 0;

            int minIndex = Mathf.Max(0, currentIndex - 1);
            int maxIndex = currentIndex + 1;

            _removeBuffer.Clear();
            foreach (var pair in _activeSegments) {
                if (pair.Key < minIndex || pair.Key > maxIndex)
                    _removeBuffer.Add(pair.Key);
            }

            for (int i = 0; i < _removeBuffer.Count; i++) {
                int index = _removeBuffer[i];
                _pool.Despawn(_activeSegments[index]);
                _activeSegments.Remove(index);
            }

            for (int index = minIndex; index <= maxIndex; index++) {
                if (_activeSegments.ContainsKey(index))
                    continue;

                SpawnSegment(index);
            }
        }

        private void SpawnSegment(int index) {
            float height = _config.SegmentHeight;
            Vector3 axisPosition = _orbitAxis != null ? _orbitAxis.position : Vector3.zero;
            Vector3 worldPosition = new Vector3(
                axisPosition.x,
                axisPosition.y + index * height + height * 0.5f,
                axisPosition.z);

            ResolveStartAnchor(index, out float startAngle, out float startY);

            CylinderSegmentView segment = _pool.Spawn(index, worldPosition);
            if (segment == null)
                return;

            _platformGenerator.Generate(segment, startAngle, startY, out _routeAngle, out _routeWorldY);
            _activeSegments[index] = segment;

            if (index > _highestGeneratedIndex)
                _highestGeneratedIndex = index;
        }

        private void ResolveStartAnchor(int index, out float startAngle, out float startY) {
            if (_activeSegments.TryGetValue(index - 1, out CylinderSegmentView previous)) {
                startAngle = previous.LastPlatformAngle;
                startY = previous.LastPlatformWorldY;
                return;
            }

            if (_highestGeneratedIndex >= 0 && index == _highestGeneratedIndex + 1) {
                startAngle = _routeAngle;
                startY = _routeWorldY;
                return;
            }

            Transform player = _movableModel.PlayerMovable.transform;
            Vector3 axisPosition = _orbitAxis != null ? _orbitAxis.position : Vector3.zero;
            Vector3 offset = player.position - axisPosition;
            startAngle = Mathf.Atan2(offset.z, offset.x);
            startY = Mathf.Max(player.position.y, _config.FirstPlatformHeight);

            if (index > 0)
                startY = index * _config.SegmentHeight;
        }
    }
}
