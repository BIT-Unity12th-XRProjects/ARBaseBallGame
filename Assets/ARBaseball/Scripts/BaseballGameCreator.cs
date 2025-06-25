using System.Collections.Generic;
using Unity.XR.CoreUtils;
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
    private ARPlaneManager _arPlaneManager;
    [SerializeField]
    private XROrigin _xrOrigin;

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

        Ray ray = _xrOrigin.Camera.ScreenPointToRay(touchPosition);
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        _arRaycastManager.Raycast(ray, hitResults);

        if (hitResults.Count > 0)
        {
            Vector3 spawnPosition = hitResults[0].pose.position;

            GameObject baseballGameObject = Instantiate(baseballGamePrefab, spawnPosition, Quaternion.identity);
            Vector3 direction = baseballGameObject.transform.position - _xrOrigin.Camera.transform.position;
            baseballGameObject.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            isCreated = true;

            _arPlaneManager.SetTrackablesActive(false);
            _arPlaneManager.enabled = false;
        }
    }
}
