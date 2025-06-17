using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �������� ���¸� �����մϴ�.
/// ������� �Է¿� ���� ������ ���� ���¸� ������Ʈ�ϰ�, ���� ������Ʈ�� �����մϴ�.
/// AI���� ��ȣ�ۿ��� ó���ϸ�, ������ ���۰� ���Ḧ �����մϴ�.
/// </summary>

public class GameManager : MonoBehaviour
{
    [Header("GameState")]
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameState currentGameState;  // ���� ���� ����

    [SerializeField] private PlayMode currentPlayMode;  // �÷��� ��� ���� ����

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

    private IEnumerator Start()
    {
        yield return StartCoroutine(InitializeGame());
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
                // ������ ���� ���Դϴ�.
                // ��: �÷��̾�� AI�� ���� ó���մϴ�.
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
    private IEnumerator InitializeGame()
    {
        // ���� �ʱ�ȭ ������ ���⿡ �ۼ��մϴ�.
        // ��: �÷��̾�� AI ��Ʈ�ѷ� �ʱ�ȭ, UI ���� ��
        Debug.Log("���� �ʱ�ȭ ��...");
        ChangeGameState(GameState.Initializing);
        ChangePlayMode(PlayMode.None); // �ʱ� �÷��� ��� ����

        // ���� 1
        // AR�� ���� �÷��̾�� AI�� ��ġ�� �����ϴ� ������ ���⿡ �ۼ��մϴ�.
        //yield return new WaitUntil(() => arIsReady);

        // ���� 2
        // UI�� ���� �÷��� ��带 �����ϵ��� �ȳ��մϴ�.
        yield return new WaitUntil(() => currentPlayMode != PlayMode.None);

        ChangeGameState(GameState.Ready);
        Debug.Log("���� �÷��� �غ� �Ϸ�. ���� ����: " + currentGameState);
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
        Debug.Log("���� ���°� ����Ǿ����ϴ�: " + currentGameState);
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
    }
}
