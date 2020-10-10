using System.Collections;
using UnityEngine;

public class CoroutineScript : MonoBehaviour
{
    public FTCScript FTC;
    public TPScript TP;

    private Calculate calculate;
    private Init init;
    private Render render;

    private bool animating;

    private void Start()
    {
        init = FTC.init;
        calculate = init.calculate;
        render = init.render;
    }

    private void FixedUpdate()
    {
        if (render.Animate(animating))
        {
            init.stage++;
            StartNewStage();
        }
    }

    internal void StartNewStage()
    {
        StartCoroutine(NewStage());
    }

    private IEnumerator NewStage()
    {
        animating = true;

        const int nextStage = 10, specialStage = 25;
        bool isSpecialStage = init.stage == 0 || init.stage == init.maxStage;

        if (!isSpecialStage && init.moduleId == Init.moduleIdCounter)
            FTC.Audio.PlaySoundAtTransform("nextStage", FTC.Module.transform);

        for (int i = 0; i < (isSpecialStage ? specialStage : nextStage); i++)
        {
            yield return new WaitForSeconds(0.1f);
            render.AssignRandom();
        }

        if (init.stage == init.maxStage)
            render.Assign(null, null, null, null);

        else
            calculate.Current();

        animating = false;
    }
}
