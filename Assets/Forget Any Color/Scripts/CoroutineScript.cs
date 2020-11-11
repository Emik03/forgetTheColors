using ForgetAnyColor;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles running coroutines for Forget Any Color since Unity requires it to be a GameObject.
/// </summary>
public class CoroutineScript : MonoBehaviour
{
    public FACScript FAC;
    public TPScript TP;

    internal bool animating = true, flashing, reset;

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
            init.stage += Init.modulesPerStage;
            StartNewStage();
        }

        else if (reset && FAC.Info.GetSolvedModuleNames().Where(m => !Arrays.Ignore.Contains(m)).Count() % Init.modulesPerStage != 0)
        {
            reset = false;
            StartFlash();
        }
    }

    internal void StartFlash()
    {
        if (!flashing)
            StartCoroutine(Flash());
    }

    internal void StartNewStage()
    {
        reset = true;
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
        bool isSpecialStage = init.stage / Init.modulesPerStage == 0 || init.stage / Init.modulesPerStage == init.maxStage / Init.modulesPerStage;

        render.Colorblind(render.colorblind);

        if (init.moduleId == Init.moduleIdCounter)
        {
            FAC.Audio.PlaySoundAtTransform("next" + (init.stage / Init.modulesPerStage % 4), FAC.Module.transform);
            if (init.stage != 0)
                FAC.Audio.PlaySoundAtTransform("nextStage", FAC.Module.transform);
            if (init.stage / Init.modulesPerStage == init.maxStage / Init.modulesPerStage)
                FAC.Audio.PlaySoundAtTransform("finalStage", FAC.Module.transform);
        }

        for (int i = 0; i < (isSpecialStage ? specialStage : nextStage); i++)
        {
            render.AssignRandom(false);
            yield return new WaitForSeconds(0.1f);
        }

        render.AssignRandom(true);

        if (init.stage / Init.modulesPerStage == init.maxStage / Init.modulesPerStage)
        {
            render.Assign(null, null, null, null, false);

            Debug.LogFormat("[Forget Any Color #{0}]: {1}{2}.",
                init.moduleId,
                calculate.modifiedSequences.Count > 0 ? "The remaining sequence is " : "There is no sequence. Turn the key",
                string.Join(", ", calculate.modifiedSequences.Select(x => x ? "Right" : "Left").ToArray()));
        }

        else
            calculate.Current();

        animating = false;
    }
}
