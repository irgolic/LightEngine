# LightEngine

Published as an example of how to sync gameplay with music in Unity. Extracted from rhythm-based danmaku mobile game I'm working on.

Using Unity 2018.2.7f1.

## Events

AudioEngine exposes three events:
- BeatEvent
- StepEvent
- MelodyEvent

BeatEvent triggers every beat, according to the provided BPM.  
StepEvent is additionally dependent on 'steps per beat' provided in LightMelody.  
MelodyEvent is dependent on a LightMelody component attached to the same GameObject as the AudioSource component.

The example project illustrates when events fire by showing/hiding circles on-screen.
