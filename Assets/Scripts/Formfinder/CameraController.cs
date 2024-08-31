using UnityEngine;

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

        private Vector3 lastMousePosition;
        private Vector3 currentVelocity = Vector3.zero;
        private float currentVerticalAngle = 0f;

        private void Update()
        {
            HandlePanning();
            HandleZooming();
        }

        private void HandlePanning()
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 panDirection = new Vector3(delta.x, -delta.y, 0).normalized;

                // Horizontal and vertical panning
                Vector3 panAmount = panDirection * (panSpeed * Time.deltaTime);
                Vector3 newPosition = transform.position + transform.right * panAmount.x + transform.up * panAmount.y;

                transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref currentVelocity, smoothTime);

                // Rotation around target
                float horizontalRotation = delta.x * rotationSpeed * Time.deltaTime;
                transform.RotateAround(targetPosition, Vector3.up, horizontalRotation);

                // Vertical rotation (limited)
                float verticalRotation = -delta.y * rotationSpeed * Time.deltaTime;
                currentVerticalAngle = Mathf.Clamp(currentVerticalAngle + verticalRotation, -verticalPanLimit, verticalPanLimit);
                Vector3 verticalRotationAxis = Vector3.Cross(transform.position - targetPosition, Vector3.up).normalized;
                transform.RotateAround(targetPosition, verticalRotationAxis, verticalRotation);

                // Ensure the camera is always looking at the target
                transform.LookAt(targetPosition);

                lastMousePosition = Input.mousePosition;
            }
        }

        private void HandleZooming()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                Vector3 zoomDirection = (targetPosition - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, targetPosition);
                float zoomAmount = scrollInput * zoomSpeed;
                float newDistance = Mathf.Clamp(distance - zoomAmount, minZoomDistance, maxZoomDistance);
                
                transform.position = targetPosition - zoomDirection * newDistance;
            }
        }
    }
}
