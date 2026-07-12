using System.Collections.Generic;
using UnityEngine;

namespace Features.IkFootModule.Scripts
{
    public class IkFootPlacement : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _distanceToGround;
        [SerializeField] private LayerMask _walkableMask;
        [SerializeField] private float _toePositionOffset = 0.25f;
        [SerializeField] private float _smoothSpeed = 10f;
        [SerializeField] private Vector3 _toeLocalDirection = Vector3.forward;

        private readonly Dictionary<AvatarIKGoal, Vector3> _footPositions = new();
        private readonly Dictionary<AvatarIKGoal, Quaternion> _footRotations = new();

        private readonly Dictionary<AvatarIKGoal, Vector3> _footRayOrigins = new();
        private readonly Dictionary<AvatarIKGoal, Vector3> _toeRayOrigins = new();

        private readonly Dictionary<AvatarIKGoal, Vector3> _footHitPoints = new();
        private readonly Dictionary<AvatarIKGoal, Vector3> _toeHitPoints = new();

        private readonly Dictionary<AvatarIKGoal, bool> _toeHits = new();


        private void OnAnimatorIK(int layerIndex)
        {
            ProcessFootPlacement(AvatarIKGoal.LeftFoot);
            ProcessFootPlacement(AvatarIKGoal.RightFoot);
        }


        private void ProcessFootPlacement(AvatarIKGoal foot)
        {
            //float footWeight = GetFootWeight(foot);
            float footWeight = 1;
            _animator.SetIKPositionWeight(foot, footWeight);
            _animator.SetIKRotationWeight(foot, footWeight);


            Vector3 footPosition = _animator.GetIKPosition(foot);
            Quaternion footBoneRotation = _animator.GetIKRotation(foot);
            Vector3 footForward = footBoneRotation * _toeLocalDirection;


            Ray ray = new Ray(
                footPosition + Vector3.up,
                Vector3.down
            );


            _footRayOrigins[foot] = ray.origin;


            Vector3 targetPosition = footPosition;
            Quaternion targetRotation = footBoneRotation;


            if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    _distanceToGround + 1f,
                    _walkableMask))
            {
                _footHitPoints[foot] = hit.point;


                Vector3 toeOffsetDirection = footForward.sqrMagnitude > 1e-6f
                    ? footForward.normalized
                    : transform.forward;
                Vector3 toeRayPosition = footPosition + toeOffsetDirection * _toePositionOffset;


                Ray toeRay = new Ray(
                    toeRayPosition + Vector3.up,
                    Vector3.down
                );


                _toeRayOrigins[foot] = toeRay.origin;


                bool hasToeHit = Physics.Raycast(
                    toeRay,
                    out RaycastHit toeHit,
                    _distanceToGround + 1f,
                    _walkableMask
                );


                _toeHits[foot] = hasToeHit;


                Vector3 groundPoint = hit.point;


                if (hasToeHit)
                {
                    _toeHitPoints[foot] = toeHit.point;


                    if (toeHit.point.y > hit.point.y)
                    {
                        groundPoint = toeHit.point;
                    }
                }


                targetPosition = groundPoint + hit.normal * _distanceToGround;


                if (hasToeHit)
                {
                    Vector3 footDirection = toeHit.point - hit.point;


                    footDirection = Vector3.ProjectOnPlane(
                        footDirection,
                        hit.normal
                    ).normalized;


                    if (footDirection != Vector3.zero)
                        targetRotation = Quaternion.LookRotation(footDirection, hit.normal);
                    else
                        targetRotation = AlignFootRotationToNormal(footBoneRotation, hit.normal);
                }
                else
                    targetRotation = AlignFootRotationToNormal(footBoneRotation, hit.normal);
            }


            if (!_footPositions.ContainsKey(foot))
            {
                _footPositions.Add(foot, targetPosition);
                _footRotations.Add(foot, targetRotation);
            }


            _footPositions[foot] = Vector3.Lerp(
                _footPositions[foot],
                targetPosition,
                Time.deltaTime * _smoothSpeed
            );


            _footRotations[foot] = Quaternion.Slerp(
                _footRotations[foot],
                targetRotation,
                Time.deltaTime * _smoothSpeed
            );


            _animator.SetIKPosition(
                foot,
                _footPositions[foot]
            );


            _animator.SetIKRotation(
                foot,
                _footRotations[foot]
            );
        }


        private Quaternion AlignFootRotationToNormal(Quaternion footBoneRotation, Vector3 groundNormal) {
            Vector3 projectedForward = Vector3.ProjectOnPlane(
                footBoneRotation * _toeLocalDirection,
                groundNormal
            ).normalized;

            if (projectedForward.sqrMagnitude < 1e-6f) {
                projectedForward = Vector3.ProjectOnPlane(
                    footBoneRotation * Vector3.right,
                    groundNormal
                ).normalized;
            }

            if (projectedForward.sqrMagnitude < 1e-6f) {
                projectedForward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
            }

            return Quaternion.LookRotation(projectedForward, groundNormal);
        }

        private float GetFootWeight(AvatarIKGoal foot)
        {
            return foot switch
            {
                AvatarIKGoal.LeftFoot => _animator.GetFloat("LeftFootIkWeight"),
                AvatarIKGoal.RightFoot => _animator.GetFloat("RightFootIkWeight"),
                _ => 0f
            };
        }


        private void OnDrawGizmos()
        {
            DrawFootGizmo(AvatarIKGoal.LeftFoot);
            DrawFootGizmo(AvatarIKGoal.RightFoot);
        }


        private void DrawFootGizmo(AvatarIKGoal foot)
        {
            if (_footRayOrigins.TryGetValue(foot, out Vector3 footOrigin))
            {
                Gizmos.color = Color.green;

                Gizmos.DrawLine(
                    footOrigin,
                    footOrigin + Vector3.down * (_distanceToGround + 1f)
                );


                if (_footHitPoints.TryGetValue(foot, out Vector3 footHit))
                {
                    Gizmos.DrawSphere(
                        footHit,
                        0.04f
                    );
                }
            }


            if (_toeRayOrigins.TryGetValue(foot, out Vector3 toeOrigin))
            {
                Gizmos.color = Color.yellow;

                Gizmos.DrawLine(
                    toeOrigin,
                    toeOrigin + Vector3.down * (_distanceToGround + 1f)
                );


                if (_toeHits.TryGetValue(foot, out bool hasHit) && hasHit)
                {
                    if (_toeHitPoints.TryGetValue(foot, out Vector3 toeHit))
                    {
                        Gizmos.color = Color.red;

                        Gizmos.DrawSphere(
                            toeHit,
                            0.05f
                        );
                    }
                }
            }
        }
    }
}