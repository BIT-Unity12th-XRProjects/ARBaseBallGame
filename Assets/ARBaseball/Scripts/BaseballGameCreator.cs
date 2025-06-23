using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


/// <summary>
/// AR ���� ����� �ν��ϰ� ��ġ �Է����� �ش� ��ġ�� ���� ������ �ν��Ͻ�
/// </summary>
public class BaseballGameCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject baseballGamePrefab;
    [SerializeField]
    private ARRaycastManager _arRaycastManager;
    [SerializeField]

    public bool isCreated = false;

    /// <summary>
    /// ��ũ�� ��ġ ���� �� baseball game prefab ����
    /// </summary>
    /// <param name="obj"></param>
    public void GenerateBaseballGame(Vector2 touchPosition)
    {
        if (isCreated)
            return;

        Debug.Log($"Touch : {touchPosition}");

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        _arRaycastManager.Raycast(ray, hitResults);

        if (hitResults.Count > 0)
        {
            Vector3 spawnPosition = hitResults[0].pose.position;

            Vector3 direction = Camera.main.transform.position - spawnPosition;
            direction.y = 0; // ��� ���� �����ǵ��� Y ��ǥ�� 0���� ����
            direction += new Vector3(0, 0.1f, 0); // �ణ ���� �÷��� ����

            GameObject baseballGameObject = Instantiate(baseballGamePrefab, spawnPosition, Quaternion.Euler(direction));
            //Vector3 direction = Camera.main.transform.position - baseballGameObject.transform.position;
            //baseballGameObject.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            isCreated = true;
        }
    }
}
