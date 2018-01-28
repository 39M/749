using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Timers;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public Level level;

    public GameObject sceneRoot;
    Vector3 sceneMoveOffset;
    GameObject player;
    public GameObject backgroundRoot;
    GameObject background;
    public GameObject enemy;
    public GameObject throwableRoot;
    GameObject throwable;
    public Animator enemyUpperAnimator;
    public Animator enemyLowerAnimator;

    public Slider progressBar;

    [SerializeField]
    Section currentSection;
    [SerializeField]
    int nextSectionId = 0;

    void Start()
    {
        Time.timeScale = 1;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = level.bgm;
        audioSource.pitch = 1;
        audioSource.Play();

        sceneMoveOffset = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f));
        sceneMoveOffset = new Vector3(sceneMoveOffset.x * 2, 0, 0);
        GetNextPlayer(true);

        NextSection();
    }

    bool win = false;
    void Update()
    {
        if (currentSection == null)
        {
            if (!win)
            {
                TimersManager.SetTimer(this, 0.5f, delegate
                {
                    SceneManager.LoadScene("GameWin");
                });
                win = true;
            }
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

        if ((Input.GetKeyDown(KeyCode.X)))
        {
            progressBar.DOKill();
            progressBar.value = 0;
        }
    }

    void NextSection()
    {
        if (currentSection != null && currentSection.GetType() == typeof(RepeatSection))
        {
            if (((RepeatSection)currentSection).miss)
            {
                player.GetComponent<Animator>().Play("Infected");
                progressBar.DOValue(progressBar.value + level.progressBarIncreaseValue, 1).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (progressBar.value >= 1)
                    {
                        Time.timeScale = 0;
                        audioSource.Stop();
                        audioSource.volume *= 3;
                        audioSource.PlayOneShot(level.failAudio);
                        transform.DOMove(Vector3.one, 3).OnComplete(() =>
                        {
                            Time.timeScale = 1;
                            SceneManager.LoadScene("GameOver");
                        }).SetUpdate(true);
                    }
                });
            }
        }

        currentSection = level.sectionList[nextSectionId++];

        if (currentSection.GetType() == typeof(RepeatSection))
        {
            Preview(currentSection as RepeatSection);
        }
        else if (currentSection.GetType() == typeof(MoveSection))
        {
            MoveSection section = currentSection as MoveSection;
            if (section.selfMove)
            {
                Vector3 scale = enemy.transform.localScale;
                scale.x = section.position.x > enemy.transform.position.x ? scale.x : -scale.x;
                enemy.transform.localScale = scale;
                enemy.transform.DOMove(section.position, section.endTime - section.startTime).SetEase(Ease.Linear);
            }

            if (section.sceneMove)
            {
                sceneRoot.transform.DOMove(sceneRoot.transform.position + sceneMoveOffset, section.endTime - section.startTime).SetEase(Ease.Linear);
                backgroundRoot.transform.DOMove(backgroundRoot.transform.position + sceneMoveOffset / 2, section.endTime - section.startTime).SetEase(Ease.Linear);
                GetNextPlayer();
            }
        }
        else if (currentSection.GetType() == typeof(ChangeSpeedSection))
        {
            ChangeSpeedSection section = currentSection as ChangeSpeedSection;
            Time.timeScale = section.speed;
            audioSource.pitch = section.speed;
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

                Destroy(throwable);
                if (beat.randomThrowable)
                {
                    throwable = Instantiate(level.throwablePrefabList[Random.Range(0, level.throwablePrefabList.Count)], throwableRoot.transform);
                }
                else
                {
                    throwable = Instantiate(beat.throwable, throwableRoot.transform);
                }
                Debug.Log("咳嗽");
            });
        }

        section.miss = false;
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
                Debug.Log(string.Format("按下时机准确度：【{0} of 1】", (audioSource.time - section.currentPlayBeat.time) / section.missRange));
                section.currentPlayBeat.hasClicked = true;
                if (offset <= section.hitRange || Input.GetKey(KeyCode.Z))
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

        Destroy(throwable);
        player.GetComponent<Animator>().Play("Hit");
    }

    void OnMiss(RepeatSection section)
    {
        Debug.Log("然后被喷射物糊了一脸");

        if (section.missAudioEffect != null)
        {
            audioSource.PlayOneShot(section.missAudioEffect);
        }

        Destroy(throwable);
        player.GetComponent<Animator>().Play("Miss");
        ((RepeatSection)currentSection).miss = true;
    }

    void OnNoUseClick(RepeatSection section)
    {
        Debug.Log("挡空了...");

        if (section.simpleClickAudioEffect != null)
        {
            audioSource.PlayOneShot(section.simpleClickAudioEffect);
        }
        player.GetComponent<Animator>().Play("HitFail");
    }

    void GetNextPlayer(bool init = false)
    {
        Destroy(player, 10);

        player = Instantiate(level.playerPrefabList[Random.Range(0, level.playerPrefabList.Count)],
            init ? Vector3.zero : sceneMoveOffset * -1,
            Quaternion.identity,
            sceneRoot.transform);

        if (init || (background != null && background.transform.position.x <= -sceneMoveOffset.x / 2))
        {
            var position = init ? Vector3.zero : background.transform.position - sceneMoveOffset;
            background = Instantiate(level.backgroundPrefab,
                position,
                Quaternion.identity,
                backgroundRoot.transform);
        }
    }

    void EndGame()
    {
        currentSection = null;
    }
}
