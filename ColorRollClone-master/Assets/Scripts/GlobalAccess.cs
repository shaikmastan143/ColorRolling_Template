using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAccess : Singleton<GlobalAccess>
{
    [SerializeField] private ConstantsSO constantsSO;

    public ConstantsSO ConstantsSO { get => constantsSO; set => constantsSO = value; }
}
