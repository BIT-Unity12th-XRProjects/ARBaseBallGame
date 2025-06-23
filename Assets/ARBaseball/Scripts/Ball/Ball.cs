using UnityEngine;


/// <summary>
/// ���״��� ȿ�� �����ϱ�
/// ���� ������ Ư���� �����ϴ� ������Ʈ�Դϴ�.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Pitch Settings")]
    [SerializeField] private float _lifetime = 15f;
    [SerializeField] private float _resistance = 0.1f;
    [SerializeField] private float magnusStrength = 5f;

    private Vector3 _startPosition;
    private Vector3 _direction;
    private float _force;
    private float _startTime;
    private PitchType _pitchType;

    private Vector3 _startPositionOffSet = new Vector3(0, 0f, 5f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Start()
    {
        _startTime = Time.time;
        Destroy(gameObject, _lifetime);
    }

    public void Shoot(Vector3 startPosition, Vector3 direction, float force, PitchType type)
    {
        _startPosition = startPosition + _startPositionOffSet;
        _direction = direction.normalized;
        _force = force;
        _pitchType = type;

        transform.position = _startPosition;

        rb.linearVelocity = _direction * _force;

        ApplySpin(_pitchType);
    }

    private void ApplySpin(PitchType type)
    {
        switch (type)
        {
            case PitchType.Fastball:
                rb.angularVelocity = transform.right * -30f; // Backspin
                break;
            case PitchType.Curve:
                if (Vector3.Dot(transform.forward, _direction) > 0)
                {
                    rb.angularVelocity = transform.right * 30f;
                }
                else
                {
                    rb.angularVelocity = transform.right * -30f;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_pitchType == PitchType.Curve)
        {
            Vector3 w = rb.angularVelocity;
            Vector3 v = rb.linearVelocity;

            if (w != Vector3.zero && v != Vector3.zero)
            {
                Vector3 magnusForce = Vector3.Cross(w, v).normalized * magnusStrength;
                rb.AddForce(magnusForce, ForceMode.Acceleration);
            }
        }

        rb.linearVelocity *= (1f - _resistance * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            // ���� ���� ����� ���� ó��
            rb.velocity = Vector3.zero; // �ӵ��� 0���� �ʱ�ȭ
            rb.angularVelocity = Vector3.zero; // ȸ�� �ӵ��� 0���� �ʱ�ȭ
            Destroy(gameObject, 2f); // 2�� �Ŀ� �� ����
        }
    }
}


}