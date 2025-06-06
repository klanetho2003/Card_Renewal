using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.UI.SetCanvas(gameObject, sort: false, sortOrder: -10);
        return true;
    }
}
