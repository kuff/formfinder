// Copyright (C) 2024 Peter Guld Leth

#region

using UnityEngine;

#endregion

namespace Formfinder
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float verticalPanLimit = 45f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoomDistance = 1f;
        [SerializeField] private float maxZoomDistance = 100f;
        
        private Vector3 _currentVelocity = Vector3.zero;
        private float _currentVerticalAngle;
        private Vector3 _lastMousePosition;

        private void Update()
        {
            HandlePanning();
            HandleZooming();
        }

        private void HandlePanning()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                var delta = Input.mousePosition - _lastMousePosition;
                var panDirection = new Vector3(delta.x, -delta.y, 0).normalized;

                // Horizontal and vertical panning
                var panAmount = panDirection * (panSpeed * Time.deltaTime);
                var newPosition = transform.position + transform.right * panAmount.x + transform.up * panAmount.y;

                transform.position =
                    Vector3.SmoothDamp(transform.position, newPosition, ref _currentVelocity, smoothTime);

                // Rotation around target
                var horizontalRotation = delta.x * rotationSpeed * Time.deltaTime;
                transform.RotateAround(targetPosition, Vector3.up, horizontalRotation);

                // Vertical rotation (limited)
                var verticalRotation = -delta.y * rotationSpeed * Time.deltaTime;
                _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle + verticalRotation, -verticalPanLimit,
                    verticalPanLimit);
                var verticalRotationAxis = Vector3.Cross(transform.position - targetPosition, Vector3.up).normalized;
                transform.RotateAround(targetPosition, verticalRotationAxis, verticalRotation);

                // Ensure the camera is always looking at the target
                transform.LookAt(targetPosition);

                _lastMousePosition = Input.mousePosition;
            }
        }

        private void HandleZooming()
        {
            var scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                var zoomDirection = (targetPosition - transform.position).normalized;
                var distance = Vector3.Distance(transform.position, targetPosition);
                var zoomAmount = scrollInput * zoomSpeed;
                var newDistance = Mathf.Clamp(distance - zoomAmount, minZoomDistance, maxZoomDistance);

                transform.position = targetPosition - zoomDirection * newDistance;
            }
        }
    }
}