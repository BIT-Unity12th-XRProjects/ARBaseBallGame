using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

/// <summary>
/// ������ �������� ���¸� �����մϴ�.
/// ������� �Է¿� ���� ������ ���� ���¸� ������Ʈ�ϰ�, ���� ������Ʈ�� �����մϴ�.
/// AI���� ��ȣ�ۿ��� ó���ϸ�, ������ ���۰� ���Ḧ �����մϴ�.
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;                       // �� ������
    public GameObject batterPrefab;                     // Ÿ�� ������
    public GameObject pitcherPrefab;                    // ���� ������
    private BaseballGameCreator baseballGameCreator;    // �߱� ���� ������ 
    private GameObject simulationCamera;                    // SimulationCamera

    /// <summary>
    /// todos : 
    /// 1. �÷��̾� �Է¿� ���� ���� ���¸� ������Ʈ�մϴ�.
    /// 2. AI���� ��ȣ�ۿ��� ó���մϴ�.
    /// 3. ���� ������Ʈ�� �����մϴ�.
    /// 4. ������ ���۰� ���Ḧ �����մϴ�.
    /// </summary>
    [Header("GameState")]
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameState currentGameState;// ���� ���� ����
    [SerializeField] private PlayMode currentPlayMode;  // �÷��� ��� ���� ����
    [SerializeField] private PitchType currentPitchType;

    [Header("�÷��̾�� �ν��Ͻ� ���� ���")]
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private GameObject aIGameObject;
    [SerializeField] private GameObject bat;
    [SerializeField] private GameObject baseballField;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform pitcherPosition;
    [SerializeField] private Transform batterPosition;
    [SerializeField] private Vector3 defaultPitcherPosition;

    private Vector3 cameraOffset;
    private Vector3 pitchOffset = new Vector3(-5f, -5f, 40f); 
    private Vector3 batOffset = new Vector3(10f, 0f, -40f); 


    public event Action<GameState> OnGameStateChanged;
    public event Action<PlayMode> OnPlayModeChanged;
    public event Action<int> OnRestTimeChanged;
    private float defaultRestTime = 30f;
    private float currentRestTime;
    private float elapsedTime = 0f;
    public float minForce = 10f;
    public float maxForce = 25f;
    private Vector3 pitchAnimationCameraPosition;

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
        simulationCamera = GameObject.Find("SimulationCamera");

        // ĳ���� ���� ��ǥ �ʱ�ȭ
        pitcherPosition = GameObject.Find("PitcherPosition").transform;
        batterPosition = GameObject.Find("BatterPosition").transform;
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

        if (currentGameState == GameState.Play)
        {
            // ĳ���͸� �����ϰ�
            SpawnCharacters();

            // ī�޶��� ���� ��ġ ĳ��
            cameraTransform = simulationCamera.transform;

            baseballField = GameObject.Find("BaseballField");

            targetTransform = simulationCamera.transform;
            // �÷��� ��忡 ������ġ�� �ű� ���� ���� ���ֺ��� �ؾ���
            if (currentPlayMode == PlayMode.PitcherMode)
            {
                playerGameObject.transform.position = pitcherPosition.position;
                playerGameObject.transform.rotation = pitcherPosition.rotation;
                aIGameObject.transform.position = batterPosition.position;
                aIGameObject.transform.rotation = batterPosition.rotation;
                defaultPitcherPosition = new Vector3(pitcherPosition.position.x, 0, pitcherPosition.position.z);
                targetTransform = pitcherPosition;
                cameraOffset = pitchOffset;
                ResetRestTime(); // ������ ���۵Ǹ� Ÿ�̸Ӹ� �ʱ�ȭ�մϴ�.
            }
            else if (currentPlayMode == PlayMode.BatterMode)
            {
                playerGameObject.transform.position = batterPosition.position;
                playerGameObject.transform.rotation = batterPosition.rotation;
                aIGameObject.transform.position = pitcherPosition.position;
                targetTransform = batterPosition;
                cameraOffset = batOffset;
                bat = GameObject.Find("Bat");
            }

            playerGameObject.transform.SetParent(baseballField.transform);
            aIGameObject.transform.SetParent(baseballField.transform);

            // ȸ�� ��ȯ ���� (M = TRS)
            baseballField.transform.rotation = Quaternion.LookRotation(targetTransform.forward, Vector3.up);

            // �̵� ��ȯ ��ó��
            Vector3 newPosition = cameraTransform.position - targetTransform.position + baseballField.transform.rotation * cameraOffset;
            newPosition.y = cameraOffset.y;

            if (currentPlayMode == PlayMode.PitcherMode)
            {
                pitchAnimationCameraPosition = newPosition;
                newPosition = defaultPitcherPosition;
                playerGameObject.SetActive(false);
                aIGameObject .SetActive(false);
            }

            baseballField.transform.position = newPosition;
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
        if (currentPlayMode == PlayMode.PitcherMode)
        {
            playerGameObject = pitcherPrefab;
            aIGameObject = batterPrefab;
        }
        else if (currentPlayMode == PlayMode.BatterMode)
        {
            playerGameObject = batterPrefab;
            aIGameObject = pitcherPrefab;
        }

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

    /// <summary>
    /// ���� ���°� Play�� �Ǿ��� �� ȣ��� �Լ�
    /// ���� ��忡 ���� �÷��̾� �Ǵ� AI�� ���� �Ҵ�� �����ǿ� ������
    /// </summary>
    private void SpawnCharacters()
    {
        playerGameObject = Instantiate(playerGameObject);
        aIGameObject = Instantiate(aIGameObject);
        playerGameObject.name = "Player";
        aIGameObject.name = "AI";
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

            Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * 0.5f;
            Vector3 baseDirection = cameraTransform.forward;

            float screenRatio = (float)Screen.height / Screen.width;

            float horizontalOffset = Mathf.Clamp(drag.x / Screen.width, -0.3f, 0.3f);
            float verticalOffset = Mathf.Clamp((drag.y / Screen.height) * screenRatio, -0.3f, 0.3f);

            Vector3 direction = (baseDirection
                                + cameraTransform.right * horizontalOffset
                                + cameraTransform.up * verticalOffset).normalized;

            if (currentPlayMode == PlayMode.PitcherMode)
            {
                StartCoroutine(C_Shoot(spawnPosition, direction, force));
            }
            else if (currentPlayMode == PlayMode.BatterMode)
            {
                if (bat == null)
                {
                    Debug.LogWarning("[GameManager] Player�� Bat ������Ʈ�� �����ϴ�.");
                    return;
                }

                bat.GetComponent<Bat>().Swing(direction, force);
                playerGameObject.GetComponent<AnimationContoller>().PlayAnimation("Swing");
                Debug.Log("��Ʈ�� �ֵѷ���!");
            }
        }
    }

    private IEnumerator C_Shoot(Vector3 spawnPosition, Vector3 direction, float force)
    {
        playerGameObject.SetActive(true);
        baseballField.transform.position = pitchAnimationCameraPosition;

        Animator animator = playerGameObject.GetComponent<Animator>();
        animator.SetTrigger("Shoot");

        // �ִϸ��̼� ��� - Ŭ�� ���̸� �ƴ� ���
        float shootAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(shootAnimDuration);

        // �ʵ� ����ġ ����
        baseballField.transform.position = defaultPitcherPosition;
        playerGameObject.SetActive(false);

        // �� ������
        GameObject go = Instantiate(ballPrefab, spawnPosition, cameraTransform.rotation);
        go.GetComponent<Ball>().Shoot(spawnPosition, direction, force, currentPitchType);
    }

    /// <summary>
    /// ���� ��Ʈ�� �¾��� �� ����Ǵ� ��
    /// 1, ������ ����
    /// 2. ī�޶� ��Ʈ�� ���� ���� ���� �� �� �ֵ��� ��ġ �̵�
    /// 3. 0.4�� �ڿ� ������ �ٽ� ���
    /// 4. 0.6�� ���� ���� ���ư��� ���� ī�޶� Ʈ��ŷ
    /// 5. �� �� �ٽ� ī�޶� ���� ��ġ�� ����
    /// </summary>
    private void PlayHitSequence()
    {

    }

    public void OnBallHit()
    {
        PlayHitSequence();
    }
}
