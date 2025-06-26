using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� �����ϴ� UI ��ҵ��� �����ϴ� Ŭ�����Դϴ�.
/// ���� �Ŵ����� ��ȣ�ۿ��Ͽ� UI�� ������Ʈ
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UIPrefab")]
    [SerializeField] private List<GameObject> UIPrefabs;
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    [Header("UIControll")]
    [SerializeField] private GameObject currentPlayModeUI; // ���� Ȱ��ȭ�� UI ���

    private void Awake()
    {
        // UI �������� �ʱ�ȭ�մϴ�.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // ���� �Ŵ����� �̺�Ʈ�� �����մϴ�.
        GameManager.Instance.OnGameStateChanged += ApplyGameStateInUI;
        GameManager.Instance.OnPlayModeChanged += ApplyPlayModeUI;
        GameManager.Instance.OnRestTimeChanged += UpdateSystemTimerUI;
    }

    private void OnDisable()
    {
        // ���� �Ŵ����� �̺�Ʈ ������ �����մϴ�.
        GameManager.Instance.OnGameStateChanged -= ApplyGameStateInUI;
        GameManager.Instance.OnPlayModeChanged -= ApplyPlayModeUI;
        GameManager.Instance.OnRestTimeChanged -= UpdateSystemTimerUI;
    }

    /// <summary>
    /// UI �������� �ε��ϰ� �ʱ�ȭ�մϴ�.
    // �� UI ��Ҹ� �ʱ�ȭ�ϴ� ������ ���⿡ �ۼ��մϴ�.
    /// </summary>
    public void InitializeUI()
    {
        foreach (var uiPrefab in UIPrefabs)
        {
            if (uiPrefab != null)
            {
                var instance = Instantiate(uiPrefab, transform);
                instance.name = uiPrefab.name;
                uiElements[uiPrefab.name] = instance;
                instance.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ���� ���¿� ���� UI�� ����Ī�մϴ�.
    /// </summary>
    private void ApplyPlayModeUI(PlayMode playMode)
    {
        foreach (var uiElement in uiElements.Values)
        {
            if (uiElement.name == "SystemUI")
            {
                uiElement.SetActive(true); // �ý��� UI�� �׻� Ȱ��ȭ�մϴ�.
            }
            else if (uiElement.name == playMode.ToString() + "UI")
            {
                uiElement.SetActive(true); // ���õ� �÷��� ����� UI ��Ҹ� Ȱ��ȭ�մϴ�.
                currentPlayModeUI = uiElement; // ���� Ȱ��ȭ�� UI ��Ҹ� �����մϴ�.
            }
            else
            {
                uiElement.SetActive(false); // �ٸ� UI ��Ҵ� ��Ȱ��ȭ�մϴ�.
            }
        }
    }

    /// <summary>
    /// ���� ���� �ð��� �ý��� UI�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="currentTime"></param>
    private void UpdateSystemTimerUI(int currentTime)
    {
        if (uiElements.TryGetValue("SystemUI", out GameObject systemUI))
        {
            uiElements["SystemUI"].GetComponent<SystemUI>().UpdateTextTimer(currentTime);
        }
    }

    private void UpdateUIForInitComplete(int sequenceNumber)
    {
        string target;

        switch (sequenceNumber)
        {
            case 0:
                target = "TextARInput";
                break;

            case 1:
                target = "PlayModePanel";
                break;

            case 2:
                target = "ButtonPanel";
                break;
            default:
                target = "null";
                Debug.Log("�׷� ������ �ѹ��� �����ϴ�.");
                break;
        }
        SetOnlyActiveInSystemUI(target);
    }

    private void SetOnlyActiveInSystemUI(string targetName)
    {
        if (!uiElements.TryGetValue("SystemUI", out GameObject systemUI))
        {
            return;
        }

        if (targetName == "")
        {
            Debug.Log("�׷� UI��Ҵ� �����ϴ�.");
            return;
        }

        foreach (Transform child in systemUI.transform)
        {
            bool isTarget = child.name == targetName;
            child.gameObject.SetActive(isTarget);
        }
    }

    /// <summary>
    /// ���� ����� �ý��� UI�� �����մϴ�.
    /// </summary>
    /// <param name="result"></param>
    public void ApplyTurnResult(TurnResult result, int count)
    {
        if (uiElements.TryGetValue("SystemUI", out GameObject systemUI))
        {
            uiElements["SystemUI"].GetComponent<SystemUI>().PlayTurnResultTextAnimation(result.ToString(), 0.1f);

            uiElements["SystemUI"].GetComponent<SystemUI>().ApplyTurnResult(result.ToString(), count);
        }
    }

    public void ApplyTurnSessionResult(TurnSessionResult result, int count)
    {
        if (uiElements.TryGetValue("SystemUI", out GameObject systemUI))
        {
            uiElements["SystemUI"].GetComponent<SystemUI>().PlayTurnResultTextAnimation(result.ToString(), 0.1f);
        }

        if (result == TurnSessionResult.Out)
        {
            uiElements["SystemUI"].GetComponent<SystemUI>().ApplyTurnResult(result.ToString(), count);
        }
    }

    public void RequestCommand(PitchType pitchType)
    {
        switch (pitchType)
        {
            case PitchType.None:
                GameManager.Instance.TryChangePitchType(PitchType.None);
                break;
            case PitchType.Fastball:
                GameManager.Instance.TryChangePitchType(PitchType.Fastball);
                break;
            case PitchType.Curve:
                GameManager.Instance.TryChangePitchType(PitchType.Curve);
                break;
            default:
                Debug.LogWarning("[UIManager] �������� �ʴ� ��Ī ����Դϴ�: " + pitchType);
                break;
        }
    }

    public void RequestUpdateUIForInitComplete(int sequenceNumber)
    {
        UpdateUIForInitComplete(sequenceNumber);
    }

    public void RequestCommand(PlayMode playMode)
    {
        switch (playMode)
        {
            case PlayMode.PitcherMode:
                GameManager.Instance.TryChangePlayMode(PlayMode.PitcherMode);
                break;
            case PlayMode.BatterMode:
                GameManager.Instance.TryChangePlayMode(PlayMode.BatterMode);
                break;
            default:
                Debug.LogWarning("[UIManager] �������� �ʴ� �÷��� ����Դϴ�: " + playMode);
                break;
        }
    }

    public void RequestCommand(Command command, object parameter = null)
    {
        switch (command)
        {
            case Command.Initialize:
                GameManager.Instance.TryChangeGameState(GameState.Initializing);
                break;
            case Command.ReadyGame:
                GameManager.Instance.TryChangeGameState(GameState.Ready);
                break;
            case Command.PlayGame:
                GameManager.Instance.TryChangeGameState(GameState.Play);
                break;
            case Command.EndGame:
                GameManager.Instance.TryChangeGameState(GameState.End);
                break;
            case Command.Exit:
                GameManager.Instance.TryChangeApplicationState("Exit");
                break;
            default:
                Debug.LogWarning("Unknown command: " + command);
                break;
        }
    }

    private void ApplyGameStateInUI(GameState state)
    {
        bool activeFlag = true;

        if (!uiElements.TryGetValue("SystemUI", out GameObject systemUI))
            return;

        var system = systemUI.GetComponent<SystemUI>();

        switch (state)
        {
            case GameState.Initializing:
                system.SetArInput(activeFlag);
                break;

            case GameState.Ready:
                system.SetArInput(!activeFlag);
                system.SetPlayModePanel(!activeFlag);
                system.SetButtonPanel(activeFlag);
                break;

            case GameState.Play:
                system.SetButtonPanel(!activeFlag);
                system.SetGamePlayUI(activeFlag);
                break;
        }
    }
}
