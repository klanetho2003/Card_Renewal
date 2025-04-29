using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager
{
    // public HashSet<PvpCard> PvpCards { get; } = new HashSet<PvpCard>();
    // 
    // public GameObject SpawnGameObject(Vector3 position, string prefabName)
    // {
    //     GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
    //     go.transform.position = position;
    //     return go;
    // }
    // 
    // public T Spawn<T>(Vector3 position, int templateID = 0) where T : BaseController
    // {
    //     string prefabName = typeof(T).Name;
    // 
    //     GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
    //     go.name = prefabName;
    //     go.transform.position = position;
    // 
    //     BaseController obj = go.GetComponent<BaseController>();
    // 
    //     if (obj.ObjectType == EObjectType.PvpCard)
    //     {
    //         PvpCard card = go.GetComponent<PvpCard>();
    //         PvpCards.Add(card);
    //         card.SetInfo(templateID);
    //     }
    // 
    //     return obj as T;
    // }
    // 
    // public void Despawn<T>(T obj) where T : BaseController
    // {
    //     GameObject objGameObject = obj.gameObject;
    //     if (objGameObject.IsValid() == false)
    //         return;
    // 
    //     EObjectType objectType = obj.ObjectType;
    // 
    //     if (obj.ObjectType == EObjectType.PvpCard)
    //     {
    //         PvpCard temp = obj.GetComponent<PvpCard>();
    //         PvpCards.Remove(temp);
    //     }
    // 
    //     // To Pool
    //     {
    //         string poolName = typeof(T).Name;
    //         obj.gameObject.name = poolName;
    //     }
    // 
    //     Managers.Resource.Destroy(obj.gameObject);
    // 
    // }
    // 
    // public void DespawnAllTemps()
    // {
    //     var cards = PvpCards.ToList();
    // 
    //     foreach (var card in cards)
    //         Managers.Object.Despawn(card);
    // }
}
