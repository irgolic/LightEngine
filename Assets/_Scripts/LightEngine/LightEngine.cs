using System;
using UnityEngine;

[Serializable]
public struct Prefab
{
    public string name;
    public Transform prefab;
}

[Serializable]
public struct MusicTrack
{
    public MonoBehaviour main;
    public int BPM;
}

public class LightEngine : MonoBehaviour
{
    public static LightEngine instance;

    [Header("Music")]

    [SerializeField]
    private int sampleRate;
    [SerializeField]
    private MusicTrack[] tracks;

    internal AudioEngine audioEngine;

    internal bool levelOver;
    bool loading = true;

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        InitGame();
        StartLevel();
    }

    public void StartLevel()
    {
        audioEngine.Start();
        loading = false;
    }

    internal void LevelOver()
    {
        levelOver = true;
    }

    void FixedUpdate()
    {
        if (!loading)
        {
            audioEngine.tick();
        }
    }

    void InitGame()
    {
        Debug.Log("Loading audio engine");

        // step logic
        audioEngine = new AudioEngine(tracks, sampleRate);

        Debug.Log("Load complete");
    }

    public void Restart()
    {
        Debug.Log("Restarting level");

        audioEngine.restart();
        UnPause();

        Debug.Log("Restart complete");
    }

    public void Pause()
    {
        audioEngine.pause();
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        audioEngine.unPause();
        Time.timeScale = 1.0f;
    }

    // pauses on soft close
    void OnApplicationPause(bool pause)
    {
        if (pause) Pause();
    }

}