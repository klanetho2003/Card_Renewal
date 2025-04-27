using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class BaseController : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;

    public int DataTemplateID { get; set; }

    #region Init & Disable
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected virtual void OnDisable()
    {
        Clear();
    }
    #endregion

    #region Update & FixedUpdate
    public virtual void UpdateController() { }
    void Update()
    {
        UpdateController();
    }

    public virtual void FixedUpdateController() { }
    void FixedUpdate()
    {
        FixedUpdateController();
    }
    #endregion

    #region Animation
    protected virtual void UpdateAnimation() { }


    #endregion

    protected virtual void Clear()
    {
        if (Managers.Game == null)
            return;
    }
}
