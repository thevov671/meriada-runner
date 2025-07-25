using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Config", menuName = "Configs/Create New Player Config", order = 54)]
public class PlayerConfig : ScriptableObject
{
    [field: SerializeField] public float MovementSpeed { get; private set; }
}