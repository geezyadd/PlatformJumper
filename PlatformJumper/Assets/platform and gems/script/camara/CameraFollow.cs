using UnityEngine;

namespace platform_and_gems.CameraFollow
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // The character to follow
        public Vector3 offset = new Vector3(0, 5, -20); // The distance of the camera from the character
        public float smoothSpeed = 0.125f; // The speed of camera movement smoothing
        private Vector3 desiredPosition;

        void LateUpdate()
        {
            if (target != null)
            {
                desiredPosition = target.position + offset;
                // Smoothly move the camera position towards the target
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            }
        }
    }
}
