﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TouchControl : MonoBehaviour {

    public MenuSpawner spawnMenu;
    public HexGrid hexGrid;
    public bool enableTouch = true;

    Touch? dragTouch = null;
    int layerMask = 1 << 8;
    bool hasMove = false;
    bool hasZoom = false;
    bool inMenu = false;
    float distant = 0f;
    float zoomFactor = 1.1f;

    void Awake() {

    }

    void Start() {
        //hexGrid.SetBuilding(new Vector3(165.8f, 0f, 60f), null);
        enableTouch = true;
    }

    void Update() {
        if (!enableTouch)
            return;
        if (spawnMenu.MenuShowing) {
            if (Application.platform != RuntimePlatform.Android) {
                if (Input.GetMouseButton(0) && (Input.mousePosition.y < 0.92f * Screen.height)) {
                    HandleTurretMenu(Input.mousePosition);
                }
            } else {
                foreach (Touch touch in Input.touches) {
                    HandleTurretMenu(touch.position);
                }
            }
        } else {
            if (Application.platform != RuntimePlatform.Android) {
                if (Input.GetMouseButton(0) && (Input.mousePosition.y < 0.92f * Screen.height)) {
                    HandleTap(Input.mousePosition);
                }
                float d = Input.GetAxis("Mouse ScrollWheel");
                GameObject cam = GameObject.Find("Main Camera");
                Vector3 pos = cam.transform.position;
                if (d > 0) {
                    pos.y += 20;
                }
                if (d < 0) {
                    pos.y -= 20;
                }
                if (pos.y > 800)
                    pos.y = 800;
                if (pos.y < 400)
                    pos.y = 400;
                cam.transform.position = pos;

            } else {
                if (Input.touchCount == 1) {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Began && dragTouch == null) {
                        dragTouch = touch;
                        hasMove = false;
                        hasZoom = false;
                        return;
                    }
                    if (touch.phase == TouchPhase.Ended && touch.fingerId == dragTouch.Value.fingerId) {
                        if (!hasMove && (touch.position.y < 0.92f * Screen.height)) {
                            HandleTap(touch.position);
                        }
                        dragTouch = null;
                        return;
                    }
                    if (hasZoom) {
                        hasZoom = false;
                        dragTouch = touch;
                    }
                    if (touch.phase == TouchPhase.Moved && touch.fingerId == dragTouch.Value.fingerId) {
                        hasMove = true;
                        GameObject cam = GameObject.Find("Main Camera");
                        Vector3 newPos = cam.transform.position + new Vector3(-touch.deltaPosition.x * zoomFactor, 0, -touch.deltaPosition.y * zoomFactor);
                        if (newPos.x < -460) {
                            newPos.x = -460;
                        }
                        if (newPos.x > 460) {
                            newPos.x = 460;
                        }
                        if (newPos.z > 166) {
                            newPos.z = 166;
                        }
                        if (newPos.z < -166) {
                            newPos.z = -166;
                        }
                        cam.transform.position = newPos;
                        return;
                    }
                    
                } else if (Input.touchCount >= 2) {
                    if (hasZoom) {
                        float newDist = (Input.touches[0].position - Input.touches[1].position).magnitude;
                        GameObject cam = GameObject.Find("Main Camera");
                        Vector3 pos = cam.transform.position;
                        pos.y -= newDist - distant;
                        if (pos.y > 800)
                            pos.y = 800;
                        if (pos.y < 400)
                            pos.y = 400;
                        cam.transform.position = pos;
                        distant = newDist;
                    } else {
                        distant = (Input.touches[0].position - Input.touches[1].position).magnitude;
                    }
                    hasZoom = true;
                    hasMove = true;
                }
            }
        }
    }

    void HandleTap(Vector2 position) {
        Ray inputRay = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, layerMask)) {
            Vector3 hitPoint = hit.point;
            //print(hitPoint);
            if (hexGrid.IsBuildable(hitPoint)) {
                Vector3 menuPosition = hexGrid.GetGridGlobalPosition(hitPoint);
                spawnMenu.transform.position = menuPosition;
                spawnMenu.ShowMenu();
            } else {
                GameObject turret = hexGrid.IsUpgradeable(hitPoint);
                //print(turret.name);
                if (turret) {
                    spawnMenu.transform.position = turret.transform.position;
                    spawnMenu.ShowTurretMenu(turret);
                    GameObject rangeDisplay = turret.GetComponent<TurretBase>().rangeDisplay;
                }
            }
        }
    }

    void HandleTurretMenu(Vector2 position) {
        var pointer = new PointerEventData(EventSystem.current);
        // convert to a 2D position
        pointer.position = position;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        if (raycastResults.Count > 0) {
            // Do anything to the hit objects. Here, I simply disable the first one.
            foreach (RaycastResult rr in raycastResults) {
                if (rr.gameObject.tag == "Menu Button")
                    return;
            }
        }
        spawnMenu.DestroyMenu();
    }

}
