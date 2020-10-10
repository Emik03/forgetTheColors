using System;
using UnityEngine;

public class Selectable 
{
    public Selectable(Calculate calculate, FTCScript FTC, Init init, Render render)
    {
        this.calculate = calculate;
        this.FTC = FTC;
        this.init = init;
        this.render = render;
    }

    private readonly Calculate calculate;
    private readonly FTCScript FTC;
    private readonly Init init;
    private readonly Render render;

    internal KMSelectable.OnInteractHandler Interact(byte index)
    {
        return delegate ()
        {
            var seq = calculate.modifiedSequence;

            switch (index)
            {
                case 0:
                case 1:
                    FTC.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, FTC.Selectables[index].transform);
                    FTC.Selectables[index].AddInteractionPunch();

                    if (seq.Count > 0)
                        if (seq[0] == Convert.ToBoolean(index))
                            seq.RemoveAt(0);
                        else
                            FTC.Module.HandleStrike();
                    break;

                case 2:
                    if (seq.Count == 0 && init.stage == init.maxStage)
                        FTC.Module.HandlePass();
                    else if (!render.turnKey)
                    {
                        render.turnKey = true;
                        FTC.Audio.PlaySoundAtTransform("key", FTC.Module.transform);

                        if (Application.isEditor)
                            init.fakeStage++;
                    }
                    break;
            }

            return false;
        };
    }
}
