using System;
using UnityEngine;

public abstract class Section : ScriptableObject
{
    [Tooltip("这一段的开始时间")]
    public float startTime;
    [Tooltip("这一段的结束时间")]
    public float endTime;
    [Tooltip("可交互的开始时间")]
    public float playStartTime;
    [Tooltip("可交互的结束时间")]
    public float playEndTime;
}
