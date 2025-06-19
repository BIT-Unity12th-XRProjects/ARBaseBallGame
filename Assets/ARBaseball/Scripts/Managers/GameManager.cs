using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ������ �������� ���¸� �����մϴ�.
/// ������� �Է¿� ���� ������ ���� ���¸� ������Ʈ�ϰ�, ���� ������Ʈ�� �����մϴ�.
/// AI���� ��ȣ�ۿ��� ó���ϸ�, ������ ���۰� ���Ḧ �����մϴ�.
/// </summary>
public class GameManager : MonoBehaviour
{
    // �׽�Ʈ��
    public GameObject ballPrefab;  // �� ������

    /// <summary>
    /// todos : 
    /// 1. �÷��̾� �Է¿� ���� ���� ���¸� ������Ʈ�մϴ�.
    /// 2. AI���� ��ȣ�ۿ��� ó���մϴ�.
    /// 3. ���� ������Ʈ�� �����մϴ�.
    /// 4. ������ ���۰� ���Ḧ �����մϴ�.
    /// </summary>
    [Header("GameState")]
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameState currentGameState;  // ���� ���� ����

    [SerializeField] private PlayMode currentPlayMode;  // �÷��� ��� ���� ����

    public event Action<GameState> OnGameStateChanged;
    public event Action<PlayMode> OnPlayModeChanged;
    public event Action<int> OnRestTimeChanged;
    private float defaultRestTime = 30f;
    private float currentRestTime;
    private float elapsedTime = 0f;
    //[Header("PlayerControll")]
    //public PlayerController PlayerController { get; private set; }

    //[Header("AIControll")]
    //public AIController AIController { get; private set; }


    //[Header("UIControll")]
    //public UIController UIController { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIManager.Instance.InitializeUI();
        InitializeGame();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject go = Instantiate(ballPrefab, Camera.main.transform.position, Quaternion.identity);
            go.GetComponent<Ball>().Shoot(Camera.main.transform.position, Camera.main.transform.forward, 10f, PitchType.Fastball);
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            GameObject go = Instantiate(ballPrefab, Camera.main.transform.position, Quaternion.identity);
            go.GetComponent<Ball>().Shoot(Camera.main.transform.position, Camera.main.transform.forward, 10f, PitchType.Curve);
        }
        // ���� ���¿� ���� ������Ʈ ������ ó���մϴ�.
        switch (currentGameState)
        {
            case GameState.None:
                // �ʱ� ����, ������ ���۵��� �ʾҽ��ϴ�.
                // ���� ������ ���������� ������ �� ó���� ������ ���⿡ �ۼ��մϴ�.
                return;
            case GameState.Initializing:
                // ������ �ʱ�ȭ ���Դϴ�.
                // AR�� ���� �÷��̾�� AI�� ��ġ�� �����ϰ�, 
                // ������ ���� ��带 �����ؾ� �մϴ�.
                break;
            case GameState.Ready:
                // �÷��̾ ������ ������ �غ� �Ǿ����ϴ�.
                // ��: UI�� ���� �÷��̾ ������ ������ �� �ֵ��� �ȳ��մϴ�.
                break;
            case GameState.Play:
                {
                    if (currentPlayMode == PlayMode.PitcherMode)
                    {
                        // ��ó ��忡���� ���� ������ ó���մϴ�.
                        // �÷��̾� �� ������ Ÿ�̸� �۵�
                        elapsedTime += Time.deltaTime;

                        // Ÿ�̸Ӱ� 1�� �̻� ����ϸ� ���� ���� �ð��� ���ҽ�ŵ�ϴ�.
                        if (elapsedTime >= 1f)
                        {
                            currentRestTime -= 1f;
                            elapsedTime = 0f; // Ÿ�̸� �ʱ�ȭ

                            if (currentRestTime < 0)
                            {
                                // Ÿ�̸Ӱ� 0 ���Ϸ� �������� ���� ������ �Ѿ�ϴ�.
                            }
                            OnRestTimeChanged?.Invoke((int)currentRestTime);
                        }
                    }
                    else if (currentPlayMode == PlayMode.BatterMode)
                    {
                        // ���� ��忡���� ���� ������ ó���մϴ�.
                        // ��: �÷��̾ ���� ġ�� ������ ó���մϴ�.
                    }
                    else
                    {
                        Debug.LogWarning("���� �÷��� ��尡 �������� �ʾҽ��ϴ�.");
                        // ������ ���� ���Դϴ�.
                        // ��: �÷��̾�� AI�� ���� ó���մϴ�.
                    }
                }
                break;
            case GameState.End:
                // ������ ����Ǿ����ϴ�.
                // ��: ���и� �����ϰ�, ����� ǥ���մϴ�.
                break;
            default:
                // Critical error handling or logging
                break;
        }
    }

    /// <summary>
    /// // AR�� ���� �÷��̾�� AI�� ��ġ�� �����ϰ�, ���� ������Ʈ�� �ʱ�ȭ�մϴ�.
    /// ��ġ ������ �Ϸ�Ǹ� �÷��̾�� �÷��� ��带 ������ �� �ֽ��ϴ�.
    /// �÷��� ��尡 ���õǸ� ���� ���¸� Ready�� �����մϴ�.
    /// </summary>
    private void InitializeGame()
    {
        // ���� �ʱ�ȭ ������ ���⿡ �ۼ��մϴ�.
        // ��: �÷��̾�� AI ��Ʈ�ѷ� �ʱ�ȭ, UI ���� ��
        Debug.Log("���� �ʱ�ȭ ��...");
        ChangeGameState(GameState.Initializing);
        ChangePlayMode(PlayMode.None); // �ʱ� �÷��� ��� ����
        ChangePlayMode(PlayMode.PitcherMode);
        // ���� 1
        // AR�� ���� �÷��̾�� AI�� ��ġ�� �����ϴ� ������ ���⿡ �ۼ��մϴ�.
        //yield return new WaitUntil(() => arIsReady);

        // ���� 2
        // UI�� ���� �÷��� ��带 �����ϵ��� �ȳ��մϴ�.

        ChangeGameState(GameState.Play);
        Debug.Log("���� �÷��� �غ� �Ϸ�. ���� ����: " + currentGameState);
        OnGameStateChanged?.Invoke(currentGameState);
    }

    /// <summary>
    /// ���� ���¸� �����մϴ�.
    /// </summary>
    /// <param name="newGameState"></param>
    private void ChangeGameState(GameState newGameState)
    {
        if (currentGameState == newGameState)
        {
            return; // ���� ���¿� �����ϸ� �������� ����
        }
        currentGameState = newGameState;
        if (currentGameState == GameState.Play && currentPlayMode == PlayMode.PitcherMode)
        {
            ResetRestTime(); // ������ ���۵Ǹ� Ÿ�̸Ӹ� �ʱ�ȭ�մϴ�.
        }
        Debug.Log("���� ���°� ����Ǿ����ϴ�: " + currentGameState);
        OnGameStateChanged?.Invoke(currentGameState);
    }

    /// <summary>
    /// �÷��� ��带 �����մϴ�.
    /// </summary>
    /// <param name="newMode"></param>
    private void ChangePlayMode(PlayMode newMode)
    {
        if (newMode == currentPlayMode)
        {
            return;
        }

        currentPlayMode = newMode;
        Debug.Log("�÷��� ��尡 ����Ǿ����ϴ�: " + currentPlayMode);
        OnPlayModeChanged?.Invoke(currentPlayMode);
    }

    private void ResetRestTime()
    {
        currentRestTime = defaultRestTime;
    }

    internal void TryChangeGameState(GameState state)
    {
        ChangeGameState(state);
    }
}
