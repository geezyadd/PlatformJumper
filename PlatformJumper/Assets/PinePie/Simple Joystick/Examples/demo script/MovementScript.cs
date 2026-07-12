using UnityEngine;

namespace PinePie.SimpleJoystick.Examples.DemoScript
{
    public class MovementScript : MonoBehaviour
    {
        private JoystickUIController joystickUIController;
        public float moveSpeed = 5f;
    
        void Start()
        {
            JoystickUIController[] joysticks = FindObjectsOfType<JoystickUIController>();
            foreach (var joystick in joysticks)
            {
                if (joystick.name == "PinePie Joystick") joystickUIController = joystick;
            }
        }
    
        void Update()
        {
            transform.position +=
                moveSpeed * Time.deltaTime * (Vector3)joystickUIController.InputDirection;
        }
    }
    
}