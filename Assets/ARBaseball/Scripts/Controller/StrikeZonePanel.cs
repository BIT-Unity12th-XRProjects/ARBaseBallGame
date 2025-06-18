using UnityEngine;


/// <summary>
/// ��Ʈ����ũ���� ������ ���� ��ġ�� �˷��ֱ� ���� UI
/// </summary>
public class StrikeZonePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject ballIndicatorUI;

    private Canvas _canvas;
    private Collider _collider;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //ball object �Ǻ�
        //if (other.TryGetComponent<BallController>(out BallController ball))
        {
            GameObject indicatorObj = Instantiate(ballIndicatorUI);
            indicatorObj.transform.SetParent(_canvas.transform, false);
            indicatorObj.transform.position = _collider.ClosestPoint(other.transform.position);
        }
    }
}
