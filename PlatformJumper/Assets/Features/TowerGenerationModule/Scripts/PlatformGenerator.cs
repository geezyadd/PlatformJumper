using UnityEngine;

namespace Features.TowerGenerationModule.Scripts {
    public class PlatformGenerator {
        private readonly TowerGenerationConfig _config;

        public PlatformGenerator(TowerGenerationConfig config) {
            _config = config;
        }

        public void Generate(
            CylinderSegmentView segment,
            float startAngleRadians,
            float startWorldY,
            out float endAngleRadians,
            out float endWorldY) {
            float segmentBottom = segment.Index * _config.SegmentHeight;
            float segmentTop = segmentBottom + _config.SegmentHeight;

            float angle = startAngleRadians;
            float y = Mathf.Max(startWorldY + 0.05f, segmentBottom + _config.MinDeltaY);

            if (segment.Index == 0)
                y = Mathf.Max(y, _config.FirstPlatformHeight);

            bool spawnedAny = false;

            while (y < segmentTop - 0.25f) {
                SpawnStep(segment, angle, y);
                segment.LastPlatformAngle = angle;
                segment.LastPlatformWorldY = y;
                spawnedAny = true;

                float deltaY = Random.Range(_config.MinDeltaY, _config.MaxDeltaY);
                float deltaAngle = Random.Range(-_config.MaxDeltaAngleDegrees, _config.MaxDeltaAngleDegrees)
                    * Mathf.Deg2Rad;

                y += deltaY;
                angle += deltaAngle;
            }

            if (!spawnedAny || segment.LastPlatformWorldY < segmentTop - _config.MaxDeltaY) {
                float topY = segmentTop - 0.4f;
                float topAngle = angle;
                SpawnStep(segment, topAngle, topY);
                segment.LastPlatformAngle = topAngle;
                segment.LastPlatformWorldY = topY;
            }

            endAngleRadians = segment.LastPlatformAngle;
            endWorldY = segment.LastPlatformWorldY;
        }

        private void SpawnStep(CylinderSegmentView segment, float angleRadians, float worldY) {
            PlatformKind kind = ResolveStepKind();
            if (!SpawnPlatform(segment, angleRadians, worldY, kind) && kind == PlatformKind.Wall)
                SpawnPlatform(segment, angleRadians, worldY, PlatformKind.Ledge);
        }

        private PlatformKind ResolveStepKind() {
            if (_config.WallSpawnChance <= 0f)
                return PlatformKind.Ledge;

            if (_config.GetRandomPlatformPrefab(PlatformKind.Wall) == null)
                return PlatformKind.Ledge;

            return Random.value <= _config.WallSpawnChance
                ? PlatformKind.Wall
                : PlatformKind.Ledge;
        }

        private bool SpawnPlatform(
            CylinderSegmentView segment,
            float angleRadians,
            float worldY,
            PlatformKind kind) {
            GameObject prefab = _config.GetRandomPlatformPrefab(kind);
            if (prefab == null)
                return false;

            float radius = _config.PlatformOrbitRadius;
            Vector3 localPosition = new Vector3(
                Mathf.Cos(angleRadians) * radius,
                worldY - segment.transform.position.y,
                Mathf.Sin(angleRadians) * radius);

            GameObject platform = Object.Instantiate(prefab, segment.PlatformsRoot);
            platform.transform.localPosition = localPosition;
            platform.transform.localRotation = ResolveRotation(localPosition, kind);
            return true;
        }

        private static Quaternion ResolveRotation(Vector3 localPosition, PlatformKind kind) {
            Vector3 outward = new Vector3(localPosition.x, 0f, localPosition.z);
            if (outward.sqrMagnitude < 0.0001f)
                return Quaternion.identity;

            outward.Normalize();

            if (kind == PlatformKind.Ledge)
                return Quaternion.LookRotation(outward, Vector3.up);

            Vector3 tangent = Vector3.Cross(Vector3.up, outward).normalized;
            return Quaternion.LookRotation(tangent, Vector3.up);
        }
    }
}
