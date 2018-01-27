using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    public AudioClip bgm;
    public List<Section> sectionList;

    [Tooltip("动画状态机名")]
    public List<string> animationStateNames;
    [Tooltip("动画提前时间")]
    public List<float> animationStateAdvanceTime;

    public List<GameObject> playerPrefabList;

    public List<GameObject> throwablePrefabList;
}
