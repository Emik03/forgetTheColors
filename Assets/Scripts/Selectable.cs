using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

namespace ForgetTheColors
{
    public class Selectable
    {
        public Selectable(Calculate calculate, CoroutineScript coroutine, FTCScript FTC, Init init, LegacyFTC legacy, Render render)
        {
            this.calculate = calculate;
            this.coroutine = coroutine;
            this.FTC = FTC;
            this.init = init;
            this.legacy = legacy;
            this.render = render;
        }

        internal bool strike;
        internal int stagesCompleted;

        private readonly Calculate calculate;
        private readonly CoroutineScript coroutine;
        private readonly FTCScript FTC;
        private readonly Init init;
        private readonly LegacyFTC legacy;
        private readonly Render render;

        private int keyTurned;

        internal KMSelectable.OnInteractHandler Interact(byte index)
        {
            return delegate ()
            {
                if (legacy.active || coroutine.animating)
                    return false;

                var seq = calculate.modifiedSequence;

                switch (index)
                {
                    case 0:
                    case 1:
                        FTC.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, FTC.Selectables[index].transform);
                        FTC.Selectables[index].AddInteractionPunch();

                        if (seq.Count > 0)
                            if (seq[0] == Convert.ToBoolean(index))
                            {
                                FTC.Audio.PlaySoundAtTransform("stage" + (stagesCompleted % 4), FTC.Selectables[index].transform);
                                stagesCompleted++;
                                seq.RemoveAt(0);
                            }
                            else
                            {
                                Debug.LogFormat("[Forget The Colors #{0}]: {1} was pushed during stage {2}. {3}.", init.moduleId, index == 1 ? "Right" : "Left", stagesCompleted + 1, Arrays.Lose[Rnd.Range(0, Arrays.Lose.Length)]);
                                
                                FTC.Audio.PlaySoundAtTransform("strike", FTC.Selectables[index].transform);
                                
                                strike = true;
                                FTC.Module.HandleStrike();
                            }

                        coroutine.StartFlash();
                        break;

                    case 2:
                        if (seq.Count == 0 && init.stage == init.maxStage)
                        {
                            Debug.LogFormat("[Forget The Colors #{0}]: {1}; Thanks for playing!", init.moduleId, Arrays.Win[Rnd.Range(0, Arrays.Win.Length)]);

                            FTC.Audio.PlaySoundAtTransform("keySuccess", FTC.Module.transform);
                            FTC.Audio.PlaySoundAtTransform("solved", FTC.Module.transform);
                            
                            init.solved = true;
                            FTC.Module.HandlePass();
                            break;
                        }

                        else if (!render.turnKey)
                        {
                            FTC.Audio.PlaySoundAtTransform("key", FTC.Module.transform);
                            render.turnKey = true;
                            keyTurned++;

                            if (keyTurned >= 20 && stagesCompleted == 0 && init.stage == 0)
                                init.LegacyFTC();
                        }
                        break;
                }

                return false;
            };
        }
    }
}
