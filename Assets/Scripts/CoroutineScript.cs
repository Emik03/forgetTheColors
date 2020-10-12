using ForgetTheColors;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CoroutineScript : MonoBehaviour
{
    public FTCScript FTC;
    public TPScript TP;

    internal bool animating, flashing;

    private Calculate calculate;
    private Init init;
    private Render render;

    private void Start()
    {
        init = FTC.init;
        calculate = init.calculate;
        render = init.render;
    }

    private void FixedUpdate()
    {
        const int intensity = 5;
        float x = Mathf.Sin(Time.time) * intensity, 
              z = Mathf.Cos(Time.time) * intensity;
        FTC.Gear.localRotation = Quaternion.Euler(x, 0, z);

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
            FTC.Audio.PlaySoundAtTransform("next" + (init.stage % 4), FTC.Module.transform);
            if (init.stage != 0)
                FTC.Audio.PlaySoundAtTransform("nextStage", FTC.Module.transform);
            if (init.stage == init.maxStage)
                FTC.Audio.PlaySoundAtTransform("finalStage", FTC.Module.transform);
        }

        for (int i = 0; i < (isSpecialStage ? specialStage : nextStage); i++)
        {
            render.AssignRandom(false);
            yield return new WaitForSeconds(0.1f);
        }

        render.AssignRandom(true);

        if (init.stage == init.maxStage)
        {
            Debug.LogFormat("[Forget The Colors #{0}]: The remaining sequence is {1}", init.moduleId, string.Join(", ", calculate.modifiedSequence.Select(x => x ? "Right" : "Left").ToArray()));
            render.Assign(null, null, null, null, false);
        }

        else
            calculate.Current();

        animating = false;
    }
}
