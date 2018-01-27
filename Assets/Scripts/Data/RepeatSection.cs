using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RepeatSection : Section
{
    [Tooltip("预览节拍列表")]
    public List<PreviewBeat> previewBeatList;

    [Tooltip("玩家点击节拍列表")]
    public List<PlayBeat> playBeatList;
    [Tooltip("Miss音效")]
    public AudioClip missAudioEffect;

    [Tooltip("判定成功的时间范围，+/-hitRange")]
    public float hitRange;

    [HideInInspector]
    public List<bool> hitRecordList;
    [HideInInspector]
    public int nextHitIndex;

    void asd()
    {
        
    }
}

[Serializable]
public class PreviewBeat
{
    public float time;
    public AudioClip audioEffect;
}

[Serializable]
public class PlayBeat
{
    public float time;
    public AudioClip audioEffect;
}
