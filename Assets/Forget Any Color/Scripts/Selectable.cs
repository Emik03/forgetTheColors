using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

namespace ForgetAnyColor
{
    public class Selectable
    {
        public Selectable(Calculate calculate, CoroutineScript coroutine, FACScript FAC, Init init, Render render)
        {
            this.calculate = calculate;
            this.coroutine = coroutine;
            this.FAC = FAC;
            this.init = init;
            this.render = render;
        }

        internal bool strike;
        internal int stagesCompleted;

        private readonly Calculate calculate;
        private readonly CoroutineScript coroutine;
        private readonly FACScript FAC;
        private readonly Init init;
        private readonly Render render;

        internal KMSelectable.OnInteractHandler Interact(byte index)
        {
            return delegate ()
            {
                if (coroutine.animating)
                    return false;

                var seq = calculate.modifiedSequences;

                switch (index)
                {
                    case 0:
                    case 1:
                        FAC.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, FAC.Selectables[index].transform);
                        FAC.Selectables[index].AddInteractionPunch();

                        if (seq.Count > 0)
                            if (seq[0] == Convert.ToBoolean(index))
                            {
                                FAC.Audio.PlaySoundAtTransform("stage" + (stagesCompleted % 4), FAC.Selectables[index].transform);
                                stagesCompleted++;
                                seq.RemoveAt(0);
                            }
                            else
                            {
                                Debug.LogFormat("[Forget Any Color #{0}]: {1} was pushed during stage {2}. {3}.", init.moduleId, index == 1 ? "Right" : "Left", stagesCompleted + 1, Arrays.Lose[Rnd.Range(0, Arrays.Lose.Length)]);
                                
                                FAC.Audio.PlaySoundAtTransform("strike", FAC.Selectables[index].transform);
                                strike = true;
                                FAC.Module.HandleStrike();
                            }

                        coroutine.StartFlash();
                        break;

                    case 2:
                        if (seq.Count == 0 && init.stage == init.maxStage && !init.solved)
                        {
                            Debug.LogFormat("[Forget Any Color #{0}]: {1}; Thanks for playing!", init.moduleId, Arrays.Win[Rnd.Range(0, Arrays.Win.Length)]);

                            FAC.Audio.PlaySoundAtTransform("keySuccess", FAC.Module.transform);
                            FAC.Audio.PlaySoundAtTransform("solved", FAC.Module.transform);
                            
                            init.solved = true;
                            FAC.Module.HandlePass();
                            break;
                        }

                        else if (!render.turnKey)
                        {
                            FAC.Audio.PlaySoundAtTransform("key", FAC.Module.transform);
                            render.turnKey = true;
                        }
                        break;
                }

                return false;
            };
        }
    }
}
