using UnityEngine;
using System.Collections;

public class EventExample : MonoBehaviour
{
    public AudioSource audioSource;

    public GameObject AudioBeat;
    public GUIText energy;
    public GUIText kick;
    public GUIText snare;
    public GUIText hithat;

    public GameObject genergy;
    public GameObject gkick;
    public GameObject gsnare;
    public GameObject ghithat;

    public Material matOn;
    public Material matOff;

    public bool enableEnergy;
    public bool enableHitHat;
    public bool enableKick;
    public bool enableSnare;

    public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                if (enableEnergy)
                {
                    Debug.Log(string.Format("Energy: {0}", audioSource.time));
                    StartCoroutine(showText(energy, genergy));
                }
                break;
            case BeatDetection.EventType.HitHat:
                if (enableHitHat)
                {
                    Debug.Log(string.Format("HitHat: {0}", audioSource.time));
                    StartCoroutine(showText(hithat, ghithat));
                }
                break;
            case BeatDetection.EventType.Kick:
                if (enableKick)
                {
                    Debug.Log(string.Format("Kick: {0}", audioSource.time));
                    StartCoroutine(showText(kick, gkick));
                }
                break;
            case BeatDetection.EventType.Snare:
                if (enableSnare)
                {
                    Debug.Log(string.Format("Snare: {0}", audioSource.time));
                    StartCoroutine(showText(snare, gsnare));
                }
                break;
        }
    }

    private IEnumerator showText(GUIText texto, GameObject objeto)
    {
        texto.enabled = true;
        objeto.GetComponent<Renderer>().material = matOn;
        yield return new WaitForSeconds(0.05f * 3);
        texto.enabled = false;
        objeto.GetComponent<Renderer>().material = matOff;
        yield break;
    }

    // Use this for initialization
    void Start()
    {
        //Register the beat callback function
        AudioBeat.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;
    }

}
