using UnityEngine;

namespace Features.MovableModule.Scripts {
    public class SimpleRadialMovable : RadialMovableBase {
        protected override void Update() {
            base.Update();
            if (Input.GetKey(KeyCode.D)) {
                Move(-1);
            }
            else if (Input.GetKey(KeyCode.A)) {
                Move(1);
            }
            else {
                Move(0);
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded) {
                Jump();
            }
        }
    }
}
