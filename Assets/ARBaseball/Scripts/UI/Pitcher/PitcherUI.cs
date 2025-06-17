using UnityEngine;

/// <summary>
/// ������ UI ������Ʈ�Դϴ�.
/// </summary>
public class PitcherUI : MonoBehaviour
{ 
    [SerializeField] private PitchType currentPitchType;
    [SerializeField] private PitchTypeSelector pitchTypeSelector;

    private void Awake()
    {
        pitchTypeSelector = GetComponentInChildren<PitchTypeSelector>();
        // �ʱ� ��ġ ���� ����
        currentPitchType = PitchType.None;
        pitchTypeSelector.OnPitchTypeChanged += OnPitchTypeChanged;
    }

    private void OnPitchTypeChanged(PitchType pitchType)
    {
        currentPitchType = pitchType;
    }
}
