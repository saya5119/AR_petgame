using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
 
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour {
 
  // タップ時に表示させるオブジェクト
  [SerializeField]
private GameObject _spawnObject;
 
  private ARRaycastManager _raycastManager;
  private List<ARRaycastHit> _hitResults = new List<ARRaycastHit>();
 
  void Awake() {
    _raycastManager = GetComponent<ARRaycastManager>();
  }
 
  void Update() {
    // 画面タップされた場合
    if (Input.GetMouseButtonDown(0)) {
       if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
        // nGUI上をクリックしているので処理をキャンセルする。
        return;
      }
      if (_raycastManager.Raycast(Input.GetTouch(0).position, _hitResults)) {
        // タップされた場所にオブジェクトを生成
        Instantiate(_spawnObject, _hitResults[0].pose.position, Quaternion.identity);
      }
    }
  }
}