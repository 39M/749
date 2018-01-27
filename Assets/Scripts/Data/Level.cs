using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    public AudioClip bgm;
    public List<Section> sectionList;

    [Tooltip("咳嗽动画提前时间")]
    public float coughAnimationAdvanceTime;
}
