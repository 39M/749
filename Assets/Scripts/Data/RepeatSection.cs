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
    [Tooltip("被喷射物打中的音效")]
    public AudioClip missAudioEffect;
    [Tooltip("点击但是没打中的音效")]
    public AudioClip simpleClickAudioEffect;

    [Tooltip("判定命中的时间范围，+/-hitRange")]
    public float hitRange;
    [Tooltip("判定Miss的时间范围，+/-missRange")]
    public float missRange;

    [HideInInspector]
    public int nextHitIndex;
    [HideInInspector]
    public PlayBeat currentPlayBeat;
}

[Serializable]
public class PreviewBeat
{
    public float time;
    public AudioClip audioEffect;
    public string animationState;
}

[Serializable]
public class PlayBeat
{
    public float time;
    public AudioClip audioEffect;

    [HideInInspector]
    public bool hasClicked = false;
    [HideInInspector]
    public bool hasHit = false;
}
