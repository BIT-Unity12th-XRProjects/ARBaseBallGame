using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ��ư�� �ִ� UI ��ҷ�, ����ڰ� ��ġ ������ ������ �� �ֵ��� �մϴ�.
/// ��ġ ���� ��� ��ư�� Ŭ���ϸ� �ش� ��ư�� Ȱ��ȭ�ǰ�, �ٸ� ��ư�� ��Ȱ��ȭ�˴ϴ�.
/// �ܺη� �̺�Ʈ�� �߻���ŵ�ϴ�.
/// </summary>
public class PitchTypeSelector : MonoBehaviour
{
    public enum PitchType 
    { 
        Fastball, 
        Curve,

        None
    }


    private Dictionary<Toggle, PitchType> toggleToPitchType;
    public Toggle[] toggles;

    public event Action<PitchType> OnPitchTypeChanged;

    private void Awake()
    {
        toggleToPitchType = new Dictionary<Toggle, PitchType>();
        toggles = GetComponentsInChildren<Toggle>();

        foreach (var toggle in toggles)
        {
            var type = GetPitchTypeFromName(toggle.name);
            toggleToPitchType[toggle] = type;

            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    Debug.Log($"[PitchTypeSelector] ���õ� ����: {type}");
                    OnPitchTypeChanged?.Invoke(type);
                }
                UpdateToggleColors();
            });
        }
    }

    private void Start()
    {
        // ù ��° ��� Ȱ��ȭ
        if (toggles.Length > 0)
        {
            toggles[0].isOn = true;
        }
    }
    private PitchType GetPitchTypeFromName(string name)
    {
        foreach (PitchType type in Enum.GetValues(typeof(PitchType)))
        {
            if (name.Contains(type.ToString(), StringComparison.OrdinalIgnoreCase))
                return type;
        }

        Debug.LogWarning($"Toggle �̸����� PitchType�� ������ �� �����ϴ�: {name}, �⺻�� {PitchType.None} ��ȯ");
        return PitchType.None;
    }

    private void UpdateToggleColors()
    {
        foreach (var pair in toggleToPitchType)
        {
            var toggle = pair.Key;
            var graphic = toggle.targetGraphic;

            if (graphic != null)
            {
                graphic.color = toggle.isOn ? Color.green : Color.white;
            }
        }
    }
}
