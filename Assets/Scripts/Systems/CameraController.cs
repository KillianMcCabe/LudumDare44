using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class CameraController : MonoBehaviour
    {
        public Transform FocusOnObject;

        private Vector3 focusOffset;

        // TODO: replace with some form of collider
        private const float CameraPanXRange = 80f;
        private const float CameraPanYRange = 80f;

        private const float CameraPanAcceleration = 16f;
        private const float CameraPanDeceleration = 20f;
        private const float CameraPanMaxVelocity = 30f;

        private const float CameraZoomAcceleration = 40f;
        private const float CameraZoomDeceleration = 10f;
        private const float CameraZoomMaxVelocity = 15f;

        private const float MinCameraSize = 2f;
        private const float MaxCameraSize = 8f;

        private float _zoomVelocity = 0f;
        private float _zoomNormalizedValue = 0.5f; // range of 0 - 1
        private Vector3 _velocity = Vector3.zero;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            // if (FocusOnObject != null)
            // {
            //     transform.position = new Vector3(FocusOnObject.position.x, FocusOnObject.position.y, zPos);
            // }

            if (Input.GetMouseButtonDown(1))
            {
                // TODO: allow user to click middle mouse and drag
            }

            Vector3 cameraInput = CalculateCameraMotionInput();
            UpdateMovementVelocity(cameraInput);

            UpdateZoom();

            transform.position += _velocity * Time.deltaTime;
            ClampCameraPositionToMapBounds();
        }

        private void UpdateMovementVelocity(Vector3 input)
        {
            // add camera movement to camera velocity
            _velocity += input * CameraPanAcceleration * Time.deltaTime;
            _velocity = Vector3.ClampMagnitude(_velocity, CameraPanMaxVelocity);

            // decelerate camera if no input on corresponding axis
            float decelerationMaxDelta = CameraPanDeceleration * Time.deltaTime;
            if (input.x == 0)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, decelerationMaxDelta);
            }
            if (input.y == 0)
            {
                _velocity.y = Mathf.MoveTowards(_velocity.y, 0, decelerationMaxDelta);
            }

            // kill X velocity if camera hits max X pan range
            if (transform.position.x <= -CameraPanXRange)
            {
                _velocity.x = Mathf.Clamp(_velocity.x, 0, CameraPanMaxVelocity);
            }
            else if (transform.position.x >= CameraPanXRange)
            {
                _velocity.x = Mathf.Clamp(_velocity.x, -CameraPanMaxVelocity, 0);
            }

            // kill Y velocity if camera hits max Y pan range
            if (transform.position.y <= -CameraPanYRange)
            {
                _velocity.y = Mathf.Clamp(_velocity.y, 0, CameraPanMaxVelocity);
            }
            else if (transform.position.y >= CameraPanYRange)
            {
                _velocity.y = Mathf.Clamp(_velocity.y, -CameraPanMaxVelocity, 0);
            }
        }

        private void UpdateZoom()
        {
            _zoomVelocity += -Input.mouseScrollDelta.y * CameraZoomAcceleration * Time.deltaTime;
            if (Input.mouseScrollDelta.y == 0)
            {
                _zoomVelocity = Mathf.MoveTowards(_zoomVelocity, 0, CameraZoomDeceleration * Time.deltaTime);
            }

            // clamp zoom value
            if (_zoomNormalizedValue <= 0)
            {
                _zoomNormalizedValue = 0;
                // halt further negative velocity
                _zoomVelocity = Mathf.Clamp(_zoomVelocity, 0, CameraZoomMaxVelocity);
            }
            else if (_zoomNormalizedValue >= 1)
            {
                _zoomNormalizedValue = 1;
                // halt further positive velocity
                _zoomVelocity = Mathf.Clamp(_zoomVelocity, -CameraZoomMaxVelocity, 0);
            }

            // clamp velocity within max acceptable velocity
            _zoomVelocity = Mathf.Clamp(_zoomVelocity, -CameraZoomMaxVelocity, CameraZoomMaxVelocity);

            _zoomNormalizedValue = Mathf.Clamp01(_zoomNormalizedValue + _zoomVelocity * Time.deltaTime);

            float prevSize = _camera.orthographicSize;
            _camera.orthographicSize = Mathf.Lerp(MinCameraSize, MaxCameraSize, _zoomNormalizedValue);

            // update camera position to align to new zoom value
            float sizeChange = _camera.orthographicSize - prevSize;
            float xCorrection = (Mathf.InverseLerp(0, Screen.width, Input.mousePosition.x) - 0.5f) * 2; // mouse X position in range (-1, +1)
            float yCorrection = (Mathf.InverseLerp(0, Screen.height, Input.mousePosition.y) - 0.5f) * 2; // mouse y position in range (-1, +1)

            float aspectRatio = (float)Screen.width / (float)Screen.height;
            transform.position = new Vector3(
                transform.position.x + (xCorrection * -sizeChange * aspectRatio),
                transform.position.y + (yCorrection * -sizeChange),
                transform.position.z
            );
        }

        private void ClampCameraPositionToMapBounds()
        {
            // clamp position within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -CameraPanXRange, CameraPanXRange),
                Mathf.Clamp(transform.position.y, -CameraPanYRange, CameraPanYRange),
                transform.position.z
            );
        }

        private Vector3 CalculateCameraMotionInput()
        {
            Vector3 cameraMovementInput = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                cameraMovementInput.y = 1;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                cameraMovementInput.y = -1;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                cameraMovementInput.x = 1;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                cameraMovementInput.x = -1;
            }

            return cameraMovementInput;
        }
    }
}

