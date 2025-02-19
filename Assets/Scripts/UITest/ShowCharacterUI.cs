﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCharacterUI : MonoBehaviour {

    // 简易人物视图，用于显示每个人头顶上的血条及名字
    // 一个预制体
    public SimpleHPView simpleCharacterView;
    private Camera UICamera;
    private RectTransform Canvas;

    private CharacterMono character;
    
    private SimpleHPView UI;

    private enum VisibleStatus {
        VisibleChanged,     // 当单位变得可见时，产生的枚举状态
        VisibleInChanged,   // 当单位变得不可见时，产生的枚举状态
        NoneStatus,         // 空状态
    };
    private VisibleStatus visibleStatus = VisibleStatus.NoneStatus;

    private void Start() {
        character = GetComponent<CharacterMono>();
        Canvas = GameObject.FindObjectOfType<Canvas>().transform as RectTransform;
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    private void OnDestroy() {
        if(UI!=null)
            Destroy(UI.gameObject);
    }
    private void OnDisable() {
        if (UI != null)
            OnInvisible();
    }

    // Update is called once per frame
    void Update () {
        if (character.characterModel.IsVisible && (UI == null || !UI.isActiveAndEnabled) && character.isActiveAndEnabled) {
            OnVisible();
        } else if(character.characterModel.IsVisible==false && UI != null && UI.isActiveAndEnabled) {
            OnInvisible();
        }
        if (UI!=null && UI.enabled!=false) {
            UI.transform.localPosition = WorldPointToUIPosition(transform.position);
        }
    }

    private float GetObjectYSize() {
        return GetComponent<Collider>().bounds.size.y;
    }

    private Vector2 WorldPointToUIPosition(Vector3 worldPoint) {

        // 得到单位的头顶的世界坐标
        Vector3 topWorldPosition = new Vector3(worldPoint.x,worldPoint.y+GetObjectYSize(),worldPoint.z);

        // 将单位头顶转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(topWorldPosition);

        Vector2 UIPosition;

        //  将屏幕坐标转换为UICamera下的坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas,screenPosition,UICamera,out UIPosition);

        // 头顶UI
        return UIPosition;
    }

    /// <summary>
    /// 当物体在摄像机视野内时，显示UI
    /// </summary>
    private void OnVisible() {
        if (UI==null) {
            UI = Instantiate<SimpleHPView>(simpleCharacterView,Canvas);
            UI.transform.localPosition = WorldPointToUIPosition(transform.position);
            UI.characterMono = character;
            UI.BindingContext = new SimpleCharacterViewModel();
            UI.BindingContext.Modify(character.characterModel);
        }
        UI.Reveal(true);
    }

    /// <summary>
    /// 当物体消失在摄像机视野内时,UI消失
    /// </summary>
    private void OnInvisible() {
        if (UI != null) {
            UI.Hide(true);
        }
    }
}
