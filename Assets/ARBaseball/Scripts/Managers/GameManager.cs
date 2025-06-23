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
    public GameObject ballPrefab;  // �� ������
    private BaseballGameCreator baseballGameCreator;  // �߱� ���� ������ 
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
    [SerializeField] private PitchType currentPitchType;
    
    public event Action<GameState> OnGameStateChanged;
    public event Action<PlayMode> OnPlayModeChanged;
    public event Action<int> OnRestTimeChanged;
    private float defaultRestTime = 30f;
    private float currentRestTime;
    private float elapsedTime = 0f;
    public float minForce = 10f;
    public float maxForce = 25f;

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
        baseballGameCreator = GetComponent<BaseballGameCreator>();
        UIManager.Instance.InitializeUI();
        StartCoroutine(C_InitializeGame());
    }

    private void Update()
    {
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
    private IEnumerator C_InitializeGame()
    {
        // ���� �ʱ�ȭ ������ ���⿡ �ۼ��մϴ�.
        // ��: �÷��̾�� AI ��Ʈ�ѷ� �ʱ�ȭ, UI ���� ��
        Debug.Log("���� �ʱ�ȭ ��...");
        ChangeGameState(GameState.Initializing);
        ChangePlayMode(PlayMode.None); // �ʱ� �÷��� ��� ����

        // �ʱ� 1ȸ ���� UI ������Ʈ
        OnPlayModeChanged?.Invoke(currentPlayMode);
        UIManager.Instance.RequestUpdateUIForInitComplete(0);

        // ���� 1
        // AR�� ���� �÷��̾�� AI�� ��ġ�� �����ϴ� ������ ���⿡ �ۼ��մϴ�.
        yield return new WaitUntil(()=> baseballGameCreator.isCreated);
        Debug.Log("AR BaseballGameSetUP Complete");
        UIManager.Instance.RequestUpdateUIForInitComplete(1);

        // ���� 2
        // UI�� ���� �÷��� ��带 �����ϵ��� �ȳ��մϴ�.
        yield return new WaitUntil(() => currentPlayMode != PlayMode.None);
        UIManager.Instance.RequestUpdateUIForInitComplete(2);

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
        ChangeGameState(GameState.Ready); // �÷��� ��尡 ����Ǹ� ���� ���¸� Ready�� �����մϴ�.
    }

    private void ChangePitchType(PitchType newType)
    {
        if (newType == currentPitchType)
        {
            return;
        }

        currentPitchType = newType;
        Debug.Log("��Ī ��尡 ����Ǿ����ϴ�: " + currentPitchType);
    }
    private void ResetRestTime()
    {
        currentRestTime = defaultRestTime;
    }

    public void TryChangeGameState(GameState state)
    {
        ChangeGameState(state);
    }

    public void TryChangeApplicationState(string command)
    {
        switch (command)
        {
            case "Exit":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                break;
            default:
                Debug.LogWarning("�� �� ���� ��ɾ��Դϴ�: " + command);
                break;
        }
    }

    public void TryChangePlayMode(PlayMode playMode)
    {   
        ChangePlayMode(playMode);
    }

    public void TryChangePitchType(PitchType pitchType)
    {
        ChangePitchType(pitchType);
    }

    public void ProcessInput(Vector2 StartPosition, Vector2 EndPosition, double elapsedDraggingTime)
    {
        if (currentGameState == GameState.Initializing)
        {
            // ��Ʈ����ũ �� ���� ������ ���⿡ �ۼ��մϴ�.
            if (baseballGameCreator.isCreated)
            {
                return;
            }

            baseballGameCreator.GenerateBaseballGame(EndPosition);
        }

        else if (currentGameState == GameState.Play)
        {
            Vector2 drag = EndPosition - StartPosition;
            float force = Mathf.Clamp((float)elapsedDraggingTime * 10f, minForce, maxForce);

            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            Vector3 baseDirection = Camera.main.transform.forward;

            float screenRatio = (float)Screen.height / Screen.width;

            float horizontalOffset = Mathf.Clamp(drag.x / Screen.width, -0.3f, 0.3f);
            float verticalOffset = Mathf.Clamp((drag.y / Screen.height) * screenRatio, -0.3f, 0.3f);

            Vector3 direction = (baseDirection
                                + Camera.main.transform.right * horizontalOffset
                                + Camera.main.transform.up * verticalOffset).normalized;

            if (currentPlayMode == PlayMode.PitcherMode)
            {
                GameObject go = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
                go.GetComponent<Ball>().Shoot(spawnPosition, direction, force, currentPitchType);
            }
            else if (currentPlayMode == PlayMode.BatterMode)
            {

            }
        }
    }
}
