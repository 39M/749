using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public Level level;

    Section currentSection;
    int nextSectionId = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        foreach (float t in section.previewBeatList)
        {
            Invoke("PreviewOneBeat", t - audioSource.time);
        }

        section.hitRecordList = new List<bool>();
        foreach (float t in section.playBeatList)
        {
            section.hitRecordList.Add(false);
        }
    }

    void PreviewOneBeat()
    {
        RepeatSection section = currentSection as RepeatSection;
        audioSource.PlayOneShot(section.previewAudioEffect);
        Debug.Log("Beat");
    }

    void CheckInputRepeatSection()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            RepeatSection section = currentSection as RepeatSection;
            bool hit = false;
            for (int i = 0; i < section.playBeatList.Count; i++)
            {
                if (!section.hitRecordList[i])
                {
                    if (Mathf.Abs(audioSource.time - section.playBeatList[i]) <= section.hitRange)
                    {
                        hit = true;
                        section.hitRecordList[i] = true;
                        break;
                    }
                }
            };

            Debug.Log(hit);

            if (hit)
            {
                if (section.hitAudioEffect != null)
                {
                    audioSource.PlayOneShot(section.hitAudioEffect);
                }
            }
            else
            {
                if (section.missAudioEffect != null)
                {
                    audioSource.PlayOneShot(section.missAudioEffect);
                }
            }
        }
    }

    void EndGame()
    {
        currentSection = null;
    }
}
