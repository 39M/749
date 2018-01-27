using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Timers;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public Level level;

    Section currentSection;
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
                CheckInput();
            }
        }
    }

    void NextSection()
    {
        currentSection = level.sectionList[nextSectionId++];

        if (currentSection.GetType() == typeof(RepeatSection))
        {
            Preview();
        }
    }

    void Preview()
    {
        RepeatSection section = currentSection as RepeatSection;

        foreach (var beat in section.previewBeatList)
        {
            TimersManager.SetTimer(this, beat.time - audioSource.time, delegate
            {
                PreviewOneBeat(beat);
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

    void PreviewOneBeat(PreviewBeat beat)
    {
        audioSource.PlayOneShot(beat.audioEffect);
        Debug.Log("Beat");
    }

    void CheckInput()
    {
        RepeatSection section = currentSection as RepeatSection;
        if (section.currentPlayBeat == null)
        {
            return;
        }

        if (!section.currentPlayBeat.hasClicked && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            float offset = Mathf.Abs(audioSource.time - section.currentPlayBeat.time);

            if (offset <= section.missRange)
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
            }

            if (section.nextHitIndex >= section.playBeatList.Count)
            {
                section.currentPlayBeat = null;
            }
            else
            {
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
