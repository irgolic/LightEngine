using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventTestType
{
    Step,
    Beat,
    Melody
}

public class EventTest : MonoBehaviour
{
    public EventTestType testedEvent;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (testedEvent == EventTestType.Step)
        {
            AudioEngine.instance.StepEvent += shine;
        }
        else if (testedEvent == EventTestType.Beat)
        {
            AudioEngine.instance.BeatEvent += shine;
        }
        else
        {
            AudioEngine.instance.MelodyEvent += shine;
        }
    }

    public void shine()
    {
        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }
}
