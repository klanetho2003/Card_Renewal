using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public abstract class UI_Base : InitBase
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);       // 매핑 (게임 오브젝트 전용)
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);    // 매핑

            if (objects[i] == null)
                Debug.Log($"Failed to Bind -> {names[i]}");
        }
    }

    public void BindObjects(Type type) { Bind<GameObject>(type); }
    public void BindImages(Type type) { Bind<Image>(type); }
    public void BindTexts(Type type) { Bind<TMP_Text>(type); }
    public void BindButtons(Type type) { Bind<Button>(type); }
    public void BindToggles(Type type) { Bind<Toggle>(type); }
    public void BindSliders(Type type) { Bind<Slider>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
    protected Slider GetSliders(int idx) { return Get<Slider>(idx); }
    

    public static void BindEvent(GameObject go, Action<PointerEventData> action = null, UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }

    public static void BindEvent(GameObject go, Action<PointerEventData> action1, Action<PointerEventData> action2, Action<PointerEventData> action3,
        UIEvent type1 = UIEvent.Click, UIEvent type2 = UIEvent.None, UIEvent type3 = UIEvent.None)
    {

        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type1)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action1;
                evt.OnClickHandler += action1;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action1;
                evt.OnPointerDownHandler += action1;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action1;
                evt.OnPointerUpHandler += action1;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action1;
                evt.OnDragHandler += action1;
                break;
        }

        switch (type2)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action2;
                evt.OnClickHandler += action2;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action2;
                evt.OnPointerDownHandler += action2;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action2;
                evt.OnPointerUpHandler += action2;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action2;
                evt.OnDragHandler += action2;
                break;
        }

        switch (type3)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action3;
                evt.OnClickHandler += action3;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action3;
                evt.OnPointerDownHandler += action3;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action3;
                evt.OnPointerUpHandler += action3;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action3;
                evt.OnDragHandler += action3;
                break;
        }
    }
}
