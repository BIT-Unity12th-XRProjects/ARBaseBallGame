using UnityEngine;

/// <summary>
/// ������ UI ������Ʈ�Դϴ�.
/// </summary>
public class PitcherUI : UIBehaviour
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

    /// <summary>
    /// ��� �̺�Ʈ �߻����� ���� ������ ����
    /// </summary>
    /// <param name="pitchType"></param>
    private void OnPitchTypeChanged(PitchType pitchType)
    {
        currentPitchType = pitchType;
        UIManager.Instance.RequestCommand(currentPitchType);
    }

}
