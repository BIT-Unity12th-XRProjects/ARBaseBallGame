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
    public InputActionReferences _inputActionReferences;

    private bool _isDragging;
    private double _beginDragTimeMark;
    private Vector2 _touchStartPosition;
    private Vector2 _touchEndPosition;

#if UNITY_EDITOR
    private void Update()
    {
        if (Mouse.current == null)
            return;

        // ���콺 ��ư ����
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _touchStartPosition = Mouse.current.position.ReadValue();
            _beginDragTimeMark = Time.timeAsDouble;
        }

        // ���콺 ��ư ��
        if (_isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
            _touchEndPosition = Mouse.current.position.ReadValue();

            double elapsedDraggingTime = Time.timeAsDouble - _beginDragTimeMark;

            GameManager.Instance.ProcessInput(_touchStartPosition, _touchEndPosition, elapsedDraggingTime);
        }
    }
#endif

    private void OnEnable()
    {
        _inputActionReferences.screenTapPosition.action.performed += OnTouchPositionPerformed;
        _inputActionReferences.screenTapPosition.action.Enable();
        _inputActionReferences.screenTap.action.started += OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.canceled += OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.Enable();
    }

    private void OnDisable()
    {
        _inputActionReferences.screenTapPosition.action.performed -= OnTouchPositionPerformed;
        _inputActionReferences.screenTapPosition.action.Disable();
        _inputActionReferences.screenTap.action.started -= OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.canceled -= OnTouchPressPerformed;
        _inputActionReferences.screenTap.action.Disable();
    }

    void OnTouchPositionPerformed(InputAction.CallbackContext context)
    {
        _touchEndPosition = context.ReadValue<Vector2>();
        Debug.Log("��ġ ������");
    }

    void OnTouchPressPerformed(InputAction.CallbackContext context)
    {
        // ��ġ ����
        if (context.ReadValueAsButton())
        {
            Debug.Log("�巡�� ����");
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
            Debug.Log("�巡�� ��");
            if (_isDragging)
            {
                _isDragging = false;
                double elapsedDraggingTime = context.time - _beginDragTimeMark; // �巡�� �� �ð�

                GameManager.Instance.ProcessInput(_touchStartPosition, _touchEndPosition, elapsedDraggingTime);
            }
        }
    }
}
