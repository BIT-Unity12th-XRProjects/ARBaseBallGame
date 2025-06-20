using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �׻� Ȱ��ȭ �Ǿ� �ִ� 
/// ���� �ý����� UI�� �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class SystemUI : UIBehaviour, IButtonInteractable
{
    // �������̽� IExecuteable�� ����
    enum CommandType 
    {
        None,
        GamePlay,
        GameExit
    }

    public GameObject turnResultPrefab;

    [Header("ButtonPanelControl")]
    public GameObject buttonPanel;

    [Header("PlayModePanelControl")]
    public GameObject PlayModePanel;

    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textTurnResult;
    
    [SerializeField] private Transform ballContainer;
    [SerializeField] private Transform strikeContainer;
    [SerializeField] private Transform outContainer;

    private List<Transform> _ball = new List<Transform>();
    private List<Transform> _strike = new List<Transform>();
    private List<Transform> _out = new List<Transform>();

    private int[] _gOCounts = { 4, 3, 3 };

    [SerializeField] private Color ballColor = Color.yellow;
    [SerializeField] private Color strikeColor = new Color(1f, 0.5f, 0f); // orange
    [SerializeField] private Color outColor = Color.red;
    private Color[] _colors;

    private static int colorIndex = 0;

    private void Awake()
    {
        GameObject turnSessionResult = GameObject.Find("PanelTurnSessionResult");
        _colors = new Color[] { ballColor, strikeColor, outColor };

        InitResultIcons(ballContainer, _gOCounts[0], _ball);
        InitResultIcons(strikeContainer, _gOCounts[1], _strike);
        InitResultIcons(outContainer, _gOCounts[2], _out);
    }

    private void InitResultIcons(Transform parent, int count, List<Transform> list)
    {
        list.Clear();
        for (int i = 0; i < count; i++)
        {
            var icon = Instantiate(turnResultPrefab, parent);
            icon.name = $"{parent.name}_Icon_{i}";
            var image = icon.GetComponent<Image>();
            if (image != null)
            {
                image.color = _colors[colorIndex];
            }
            list.Add(icon.transform);
            icon.gameObject.SetActive(false); // Initially hide the icons
        }
        colorIndex += 1; // Increment color index for next icon type
    }

    public void ApplyTurnResult(string name, int count)
    {
        int index = 0;
        List<Transform> targetList = null;
        switch (name)
        {
            case "Ball":
                targetList = _ball;
                index = count - 1;
                break;
            case "Strike":
                targetList = _strike;
                index = count - 1;
                break;
            case "Out":
                targetList = _out;
                index = count - 1;
                break;
        }
        for (int i = 0; i < targetList.Count; i++)
        {
            if (i < count)
            {
                targetList[i].gameObject.SetActive(true);
            }
            else
            {
                targetList[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateTextTimer(int time)
    {
        textTimer.text = time.ToString();
    }

    public void PlayTurnResultTextAnimation(string message, float duration)
    {
        StartCoroutine(C_PlayTurnResultTextAnimation(message, duration));
    }

    /// <summary>
    /// �ؽ�Ʈ�� �ִϸ��̼��� ���� ǥ�õ˴ϴ�.
    /// 1. �ѱ��ھ� �þ�ϴ�.
    /// 2. ������ �ð� ���� ǥ�õ˴ϴ�.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator C_PlayTurnResultTextAnimation(string message, float duration)
    {
        textTurnResult.text = "";
        char[] chars = message.ToCharArray();
        
        for (int i = 0; i < chars.Length; ++i)
        {
            textTurnResult.text += chars[i];
            yield return new WaitForSeconds(duration);
        }

        yield return new WaitForSeconds(1f); 
        textTurnResult.text = "";
    }

    public void OnButtonClicked()
    {
        string buttonName = ButtonParser();

        if (buttonName == "PitcherMode" || buttonName == "BatterMode")
        {
            // PlayMode ���������� ��ȯ�մϴ�.
            if (Enum.TryParse<PlayMode>(buttonName, out PlayMode playMode))
            {
                Debug.Log($"{playMode} ��ư Ŭ����");
                UIManager.Instance.RequestCommand(playMode);
            }
        }
        // CommandType ���������� ��ȯ�մϴ�.
        else if (Enum.TryParse<Command>(buttonName, out Command command))
        {
            Debug.Log($"{ command } ��ư Ŭ����");
            UIManager.Instance.RequestCommand(command);
        }
        else
        {
            Debug.LogWarning($"[SystemUI] '{buttonName}'�� ��ȿ�� CommandType�� �ƴմϴ�.");
        }
    }

    public void SetButtonPanel(bool flag)
    {
        buttonPanel.SetActive(flag);
    }

    public void SetPlayModePanel(bool flag)
    {
        PlayModePanel.SetActive(flag);
    }
}
