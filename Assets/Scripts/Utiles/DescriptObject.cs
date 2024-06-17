using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DescriptObject", menuName = "DescriptObject",order = int.MaxValue)]
public class DescriptObject : ScriptableObject
{
    [SerializeField] private string description;
    public string Description => description;
}
