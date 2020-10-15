﻿using ForgetAnyColor;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CoroutineScript : MonoBehaviour
{
    public FACScript FAC;
    public TPScript TP;

    internal bool animating = true, flashing;

    private Calculate calculate;
    private Init init;
    private Render render;

    private void Start()
    {
        init = FAC.init;
        calculate = init.calculate;
        render = init.render;
    }

    private void FixedUpdate()
    {
        const int intensity = 5;
        float x = Mathf.Sin(Time.time) * intensity, 
              z = Mathf.Cos(Time.time) * intensity;
        FAC.Gear.localRotation = Quaternion.Euler(x, 0, z);

        if (render.Animate(animating))
        {
            init.stage++;
            StartNewStage();
        }
    }

    internal void StartFlash()
    {
        if (!flashing)
            StartCoroutine(Flash());
    }

    internal void StartNewStage()
    {
        animating = true;
        StartCoroutine(NewStage());
    }

    private IEnumerator Flash()
    {
        flashing = true;

        const int flash = 2;
        for (int i = 0; i < flash; i++)
        {
            render.AssignRandom(false);
            yield return new WaitForSeconds(0.1f);
        }

        render.Assign(null, null, null, null, false);
        render.SetNixieAsInputs();

        flashing = false;
    }

    private IEnumerator NewStage()
    {
        const int nextStage = 5, specialStage = 20;
        bool isSpecialStage = init.stage == 0 || init.stage == init.maxStage;

        render.Colorblind(render.colorblind);

        if (init.moduleId == Init.moduleIdCounter)
        {
            FAC.Audio.PlaySoundAtTransform("next" + (init.stage % 4), FAC.Module.transform);
            if (init.stage != 0)
                FAC.Audio.PlaySoundAtTransform("nextStage", FAC.Module.transform);
            if (init.stage == init.maxStage)
                FAC.Audio.PlaySoundAtTransform("finalStage", FAC.Module.transform);
        }

        for (int i = 0; i < (isSpecialStage ? specialStage : nextStage); i++)
        {
            render.AssignRandom(false);
            yield return new WaitForSeconds(0.1f);
        }

        render.AssignRandom(true);

        if (init.stage == init.maxStage)
        {
            Debug.LogFormat("[Forget Any Color #{0}]: {1}{2}.", init.moduleId, calculate.modifiedSequences.Count > 0 ? "The remaining sequence is " : "There is no sequence. Turn the key", string.Join(", ", calculate.modifiedSequences.Select(x => x ? "Right" : "Left").ToArray()));
            render.Assign(null, null, null, null, false);
        }

        else
            calculate.Current();

        animating = false;
    }
}