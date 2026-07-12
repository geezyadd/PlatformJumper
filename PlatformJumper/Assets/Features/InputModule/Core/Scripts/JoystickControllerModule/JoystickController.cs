using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Features.InputModule.Core.Scripts.JoystickControllerModule {
    /// <summary>
    /// UI joystick that feeds a Vector2 into a selected Input Action binding via OnScreenControl.
    /// Assign an <see cref="InputActionReference"/>; the control path is taken from that action's stick binding,
    /// or from the manual override below. The action must have a matching Vector2 binding (e.g. &lt;Gamepad&gt;/leftStick).
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class JoystickController : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        [Header("Input")]
        [SerializeField] private InputActionReference _action;
        [SerializeField, InputControl(layout = "Vector2")]
        private string _controlPathOverride = "<Gamepad>/leftStick";

        [Header("Visuals")]
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;

        [Header("Settings")]
        [SerializeField, Range(0f, 2f)] private float _handleRange = 1f;
        [SerializeField, Range(0f, 1f)] private float _deadZone;

        private Canvas _canvas;
        private Camera _uiCamera;
        private Vector2 _input;
        private string _resolvedControlPath;

        public Vector2 Direction => _input;
        public float Horizontal => _input.x;
        public float Vertical => _input.y;

        protected override string controlPathInternal {
            get => _resolvedControlPath;
            set => _resolvedControlPath = value;
        }

        private void Awake() {
            _canvas = GetComponentInParent<Canvas>();
            if (_background == null)
                _background = (RectTransform)transform;

            ApplyControlPath();
        }

        private void OnValidate() {
            _resolvedControlPath = ResolveDesiredPath();
        }

        public void OnPointerDown(PointerEventData eventData) {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_canvas == null)
                return;

            ResolveCamera();

            Vector2 radius = _background.sizeDelta / 2f;
            Vector2 backgroundScreen = RectTransformUtility.WorldToScreenPoint(_uiCamera, _background.position);
            Vector2 raw = (eventData.position - backgroundScreen) / (radius * _canvas.scaleFactor);

            if (raw.magnitude > 1f)
                raw = raw.normalized;

            _input = raw.magnitude < _deadZone ? Vector2.zero : raw;

            if (_handle != null)
                _handle.anchoredPosition = _input * radius * _handleRange;

            SendValueToControl(_input);
        }

        public void OnPointerUp(PointerEventData eventData) {
            _input = Vector2.zero;
            if (_handle != null)
                _handle.anchoredPosition = Vector2.zero;
            SendValueToControl(Vector2.zero);
        }

        protected override void OnDisable() {
            _input = Vector2.zero;
            if (_handle != null)
                _handle.anchoredPosition = Vector2.zero;
            SendValueToControl(Vector2.zero);
            base.OnDisable();
        }

        private void ApplyControlPath() {
            string desiredPath = ResolveDesiredPath();
            if (controlPath == desiredPath)
                return;

            controlPath = desiredPath;
        }

        private string ResolveDesiredPath() {
            string fromAction = TryResolvePathFromAction(_action);
            return string.IsNullOrEmpty(fromAction) ? _controlPathOverride : fromAction;
        }

        private void ResolveCamera() {
            _uiCamera = null;
            if (_canvas != null && _canvas.renderMode == RenderMode.ScreenSpaceCamera)
                _uiCamera = _canvas.worldCamera;
        }

        private static string TryResolvePathFromAction(InputActionReference actionReference) {
            InputAction action = actionReference?.action;
            if (action == null)
                return null;

            string fallback = null;
            foreach (InputBinding binding in action.bindings) {
                if (binding.isComposite || binding.isPartOfComposite)
                    continue;

                string path = binding.effectivePath;
                if (string.IsNullOrEmpty(path))
                    continue;

                if (path.IndexOf("Stick", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("dpad", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return path;

                fallback ??= path;
            }

            return fallback;
        }
    }
}
