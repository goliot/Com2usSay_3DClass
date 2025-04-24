using System.Collections.Generic;
using UnityEngine;

public class CommonPoolManager : BasePoolManager<EObjectType, CommonPoolInfo>
{
    private static CommonPoolManager _instance;
    public static CommonPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<CommonPoolManager>();
            }
            return _instance;
        }
    }
}