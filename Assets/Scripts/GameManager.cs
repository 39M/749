using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Timers;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public Level level;

    public GameObject player;
    public GameObject enemy;
    public Animator enemyUpperAnimator;
    public Animator enemyLowerAnimator;

    [SerializeField]
    Section currentSection;
    [SerializeField]
    int nextSectionId = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = level.bgm;
        audioSource.Play();

        NextSection();
    }

    void Update()
    {
        if (currentSection == null)
        {
            return;
        }

        if (audioSource.time > currentSection.endTime)
        {
            if (nextSectionId >= level.sectionList.Count)
            {
                EndGame();
                return;
            }
            else
            {
                NextSection();
            }
        }

        if (currentSection.playStartTime <= audioSource.time && audioSource.time <= currentSection.playEndTime)
        {
            if (currentSection.GetType() == typeof(RepeatSection))
            {
                CheckInput(currentSection as RepeatSection);
            }
        }
    }

    void NextSection()
    {
        currentSection = level.sectionList[nextSectionId++];

        if (currentSection.GetType() == typeof(RepeatSection))
        {
            Preview(currentSection as RepeatSection);
        }
        else if (currentSection.GetType() == typeof(MoveSection))
        {
            MoveSection section = currentSection as MoveSection;
            Vector3 scale = enemy.transform.localScale;
            scale.x = section.position.x > enemy.transform.position.x ? 1 : -1;
            enemy.transform.localScale = scale;
            enemy.transform.DOMove(section.position, section.endTime - section.startTime).SetEase(Ease.Linear);
        }
    }

    void Preview(RepeatSection section)
    {
        foreach (var beat in section.previewBeatList)
        {
            if (!string.IsNullOrEmpty(beat.animationState))
            {
                float animAdvancedTime = level.animationStateAdvanceTime[level.animationStateNames.IndexOf(beat.animationState)];
                TimersManager.SetTimer(this, beat.time - audioSource.time - animAdvancedTime, delegate
                {
                    enemyUpperAnimator.Play(beat.animationState);
                });
            }

            TimersManager.SetTimer(this, beat.time - audioSource.time, delegate
            {
                audioSource.PlayOneShot(beat.audioEffect);
                Debug.Log("咳嗽");
            });
        }

        section.currentPlayBeat = section.playBeatList[0];
        section.nextHitIndex = 1;
        foreach (var beat in section.playBeatList)
        {
            beat.hasClicked = false;
            beat.hasHit = false;
        }
    }

    void CheckInput(RepeatSection section)
    {
        if (section.currentPlayBeat == null || section.nextHitIndex > section.playBeatList.Count)
        {
            return;
        }

        if (!section.currentPlayBeat.hasClicked && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            float offset = Mathf.Abs(audioSource.time - section.currentPlayBeat.time);

            if (offset < section.missRange)
            {
                section.currentPlayBeat.hasClicked = true;
                if (offset <= section.hitRange)
                {
                    OnHit(section.currentPlayBeat);
                }
                else
                {
                    OnNoUseClick(section);
                }
                return;
            }
            OnNoUseClick(section);
        }

        if (audioSource.time - section.currentPlayBeat.time > section.missRange)
        {
            if (!section.currentPlayBeat.hasHit)
            {
                OnMiss(section);
                //Debug.Log(string.Format("{0} {1} {2}", audioSource.time, section.currentPlayBeat.time, section.nextHitIndex));
            }

            if (section.nextHitIndex >= section.playBeatList.Count)
            {
                //Debug.Log("process to null");
                section.currentPlayBeat = null;
                section.nextHitIndex++;
                //Debug.Log(string.Format("{0} {1}", section.currentPlayBeat, section.currentPlayBeat == null));
            }
            else
            {
                //Debug.Log(string.Format("process to next beat {0}", section.nextHitIndex));
                section.currentPlayBeat = section.playBeatList[section.nextHitIndex++];
            }
        }
    }

    void OnHit(PlayBeat beat)
    {
        Debug.Log("成功挡住！");

        beat.hasHit = true;
        if (beat.audioEffect != null)
        {
            audioSource.PlayOneShot(beat.audioEffect);
        }
    }

    void OnMiss(RepeatSection section)
    {
        Debug.Log("然后被喷射物糊了一脸");

        if (section.missAudioEffect != null)
        {
            audioSource.PlayOneShot(section.missAudioEffect);
        }
    }

    void OnNoUseClick(RepeatSection section)
    {
        Debug.Log("挡空了...");

        if (section.simpleClickAudioEffect != null)
        {
            audioSource.PlayOneShot(section.simpleClickAudioEffect);
        }
    }

    void EndGame()
    {
        currentSection = null;
    }
}
