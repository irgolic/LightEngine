using System;
using UnityEngine;


public class AudioEngine
{
    public static AudioEngine instance;
    public event Action StepEvent;
    public event Action BeatEvent;
    public event Action MelodyEvent;

    MusicTrack[] tracks;
    int currentTrackNum;
    AudioSource previouslyPlaying;
    AudioSource nowPlaying;
    AudioSource queued;

    LightMelody melody;

    int stepLength;
    int stepCount;
    int stepsPerBeat;
    int sampleRate;
    int suggestOnStepNumber;

    bool loading = true;
    public AudioEngine(MusicTrack[] tracks, int sampleRate)
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        this.sampleRate = sampleRate;
        this.tracks = tracks;

        init();

        loading = false;
    }

    void init()
    {
        currentTrackNum = 0;

        reinit(tracks[0]);

        // initial AudioSources load
        nowPlaying = tracks[0].main.GetComponent<AudioSource>();
        nowPlaying.clip.LoadAudioData(); // load first main track
        if (tracks.Length > 1)
        {
            queued = tracks[1].main.GetComponent<AudioSource>();
            queued.clip.LoadAudioData(); // load first boss intro
        }
    }

    void reinit(MusicTrack track)
    {
        // reset parameters
        stepCount = 0;

        melody = track.main.GetComponent<LightMelody>();
        if (melody != null)
        {
            stepsPerBeat = melody.stepsPerBeat;
        }
        else
        {
            stepsPerBeat = 1;
        }

        stepLength = (sampleRate * 60) / (track.BPM * melody.stepsPerBeat);

        float mainLengthSamples = track.main.GetComponent<AudioSource>().clip.length * sampleRate;
        suggestOnStepNumber = (int)Mathf.Floor((mainLengthSamples - stepLength) / stepLength);
    }

    public void tick()
    {
        if (loading) return;

        // if new track/track looped
        if (nowPlaying.timeSamples <= stepLength * (stepCount - 1))
        {
            stepCount = 0;
        }

        // if too soon for step,
        if (nowPlaying.timeSamples <= stepLength * stepCount)
        {
            return;
        }

        // STEP!
        stepCount++;

        // if first beat of track
        if (stepCount == 1 && previouslyPlaying != null)
        {
            // unload previous track
            previouslyPlaying.clip.UnloadAudioData();
            previouslyPlaying = null;
        }

        // switch to next audio source
        if (stepCount == suggestOnStepNumber)
        {
            SuggestNextAudioSource();
            suggestOnStepNumber = -1;
        }

        // if melody
        if (melody != null && melody.isStepMelody() && MelodyEvent != null)
            MelodyEvent();

        // if on beat
        if ((stepCount - 1) % stepsPerBeat == 0 && BeatEvent != null)
        {
            BeatEvent();
        }

        if (StepEvent != null)
            StepEvent();
    }

    public void Start()
    {
        nowPlaying.Play();
    }

    // sets scheduled start/end time for audio sources
    // called one step before main beat
    public void SuggestNextAudioSource()
    {
        if (queued == null)
        {
            return;
        }

        int samplesUntilNextStep = stepLength - (nowPlaying.timeSamples % stepLength);

        double timeToWait = samplesUntilNextStep / (double)sampleRate;
        double scheduledTime = AudioSettings.dspTime + timeToWait;

        Debug.Log("Scheduling new track...");
        queued.PlayScheduled(scheduledTime);
        nowPlaying.SetScheduledEndTime(scheduledTime);
        queueNextAudioSource();
    }

    void queueNextAudioSource()
    {
        // switch to next audiosource
        previouslyPlaying = nowPlaying;
        nowPlaying = queued;

        // load and queue next track
        if (currentTrackNum + 1 < tracks.Length)
        {
            currentTrackNum++;
            reinit(tracks[currentTrackNum]);
            if (tracks.Length - 1 == currentTrackNum)
            {
                queued = tracks[currentTrackNum + 1].main.GetComponent<AudioSource>();
                queued.clip.LoadAudioData();
            }
            return;
        }

        // nothing else to load
        queued = null;
    }

    internal void pause()
    {
        nowPlaying.Pause();
        queued.Pause();
    }

    internal void unPause()
    {
        nowPlaying.UnPause();
        queued.UnPause();
    }

    internal void restart()
    {
        loading = true;

        nowPlaying.Stop();
        queued.Stop();
        nowPlaying.clip.UnloadAudioData();
        queued.clip.UnloadAudioData();

        init();

        loading = false;
    }

    public float GetBpsFactor()
    {
        return tracks[currentTrackNum].BPM / 60f;
    }
}