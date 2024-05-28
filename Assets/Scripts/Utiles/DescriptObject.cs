using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DescriptObject", menuName = "DescriptObject",order = int.MaxValue)]
public class DescriptObject : ScriptableObject
{
    [SerializeField] private string description;
    public string Description => description;
}
