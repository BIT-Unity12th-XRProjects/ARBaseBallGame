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
        GameManager.Instance.OnPlayModeChanged += ApplyPlayModeUI;
        GameManager.Instance.OnRestTimeChanged += UpdateSystemTimerUI;
    }

    private void OnDisable()
    {
        // ���� �Ŵ����� �̺�Ʈ ������ �����մϴ�.
        GameManager.Instance.OnPlayModeChanged -= ApplyPlayModeUI;
    }
    private void Start()
    {
        // ���� ���� �� UI�� �ʱ�ȭ�մϴ�.
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

    private void UpdateSystemTimerUI(int currentTime)
    {
        if (uiElements.TryGetValue("SystemUI", out GameObject systemUI))
        {
            uiElements["SystemUI"].GetComponent<SystemUI>().UpdateTextTimer(currentTime);
        }
    }
}
