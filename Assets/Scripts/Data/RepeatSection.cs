using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RepeatSection : Section
{
    [Tooltip("预览节拍音效")]
    public AudioClip previewAudioEffect;
    [Tooltip("预览节拍时间点")]
    public List<float> previewBeatList;

    [Tooltip("击中音效")]
    public AudioClip hitAudioEffect;
    [Tooltip("Miss音效")]
    public AudioClip missAudioEffect;
    [Tooltip("玩家点击时间点")]
    public List<float> playBeatList;

    [HideInInspector]
    public List<bool> hitRecordList;

    [Tooltip("判定成功的时间范围，+/-hitRange")]
    public float hitRange;
}
