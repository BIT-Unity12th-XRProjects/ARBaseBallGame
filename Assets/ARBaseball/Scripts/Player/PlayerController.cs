using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation.Samples;

/// <summary>
/// ������� �Է��� ó���ϰ� �÷��̾��� �ൿ�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Input actions")]
    private InputActionReferences _inputActionReferences;

    private bool _isDragging;
    private double _beginDragTimeMark;
    private Vector2 _touchStartPosition;
    private Vector2 _touchEndPosition;

    private void OnEnable()
    {
        _inputActionReferences.screenTapPosition.action.performed += OnTouchPositionPerformed;
        _inputActionReferences.screenTap.action.canceled += OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.Enable();
    }

    private void OnDisable()
    {
        _inputActionReferences.screenTapPosition.action.performed -= OnTouchPositionPerformed;
        _inputActionReferences.screenTap.action.canceled -= OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.Disable();
    }

    void OnTouchPositionPerformed(InputAction.CallbackContext context)
    {
        _touchEndPosition = context.ReadValue<Vector2>();
    }

    void OnTouchPressPerformed(InputAction.CallbackContext context)
    {
        // ��ġ ����
        if (context.ReadValueAsButton())
        {
            if (_isDragging == false)
            {
                _isDragging = true;
                _touchStartPosition = _touchEndPosition;
                _beginDragTimeMark = context.time;
            }
        }
        // ��ġ ��
        else
        {
            if (_isDragging)
            {
                _isDragging = false;
                double elapsedDraggingTime = context.time - _beginDragTimeMark; // �巡�� �� �ð�

                GameManager.Instance.ProcessInput(_touchStartPosition, _touchEndPosition, elapsedDraggingTime);
            }
        }
    }
}
