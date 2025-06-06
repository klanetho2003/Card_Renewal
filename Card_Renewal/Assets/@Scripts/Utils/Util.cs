using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Color HexToColor(string color)
    {
        if (color.Contains("#") == false)
            color = $"#{color}";

        ColorUtility.TryParseHtmlString(color, out Color parseColor);

        return parseColor;
    }

    public static Vector3 TransformPoint(Vector3 local, Vector3 refPos, Quaternion refRot, Vector3 refScale)
    {
        // 1) 스케일
        Vector3 scaled = new Vector3(local.x * refScale.x, local.y * refScale.y, local.z * refScale.z);
        // 2) 회전
        Vector3 rotated = refRot * scaled;
        // 3) 이동
        return refPos + rotated;
    }

    public static bool IsMagnitudeEqual(Vector3 vector_A, Vector3 vector_B, float EPS = 0.01f)
    {
        return (vector_A - vector_B).sqrMagnitude < EPS * EPS;
    }

    // 갓챠
    public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
    {
        float totalWeight = sequence.Sum(weightSelector);

        double itemWeightIndex = new System.Random().NextDouble() * totalWeight;
        float currentWeightIndex = 0;

        foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
        {
            currentWeightIndex += item.Weight;

            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if (currentWeightIndex >= itemWeightIndex)
                return item.Value;

        }

        return default(T);
    }
}
