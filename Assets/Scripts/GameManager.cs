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
                CheckInputRepeatSection();
            }
        }
    }

    void NextSection()
    {
        currentSection = level.sectionList[nextSectionId++];

        if (currentSection.GetType() == typeof(RepeatSection))
        {
            PreviewRepeatSection();
        }
    }

    void PreviewRepeatSection()
    {
        RepeatSection section = currentSection as RepeatSection;

        foreach (var beat in section.previewBeatList)
        {
            TimersManager.SetTimer(this, beat.time - audioSource.time, delegate
            {
                PreviewOneBeatRepeatSection(beat);
            });
        }

        section.hitRecordList = new List<bool>();
        section.nextHitIndex = 0;
        foreach (var beat in section.playBeatList)
        {
            section.hitRecordList.Add(false);
            TimersManager.SetTimer(this, beat.time + section.hitRange - audioSource.time, delegate
            {
                CheckOneInputRepeatSection(section);
            });
        }
    }

    void PreviewOneBeatRepeatSection(PreviewBeat beat)
    {
        audioSource.PlayOneShot(beat.audioEffect);
        Debug.Log("Beat");
    }

    void CheckInputRepeatSection()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            RepeatSection section = currentSection as RepeatSection;
            bool hit = false;
            PlayBeat beat = null;
            for (int i = 0; i < section.playBeatList.Count; i++)
            {
                if (!section.hitRecordList[i])
                {
                    if (Mathf.Abs(audioSource.time - section.playBeatList[i].time) <= section.hitRange)
                    {
                        hit = true;
                        beat = section.playBeatList[i];
                        section.hitRecordList[i] = true;
                        break;
                    }
                }
            };

            if (hit)
            {
                OnHitRepeatSection(beat);
            }
            else
            {
                OnMissRepeatSection(section);
            }
        }
    }

    void CheckOneInputRepeatSection(RepeatSection section)
    {
        if (!section.hitRecordList[section.nextHitIndex++])
        {
            OnMissRepeatSection(section);
        }
    }

    void OnHitRepeatSection(PlayBeat beat)
    {
        Debug.Log("HIT!");

        if (beat.audioEffect != null)
        {
            audioSource.PlayOneShot(beat.audioEffect);
        }
    }

    void OnMissRepeatSection(RepeatSection section)
    {
        Debug.Log("MISS");

        if (section.missAudioEffect != null)
        {
            audioSource.PlayOneShot(section.missAudioEffect);
        }
    }

    void EndGame()
    {
        currentSection = null;
    }
}
