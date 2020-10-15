using ForgetAColor;
using UnityEngine;

/// <summary>
/// Forget A Color - A Keep Talking and Nobody Explodes Modded Module by Emik.
/// </summary>
public class FACScript : MonoBehaviour
{
    public CoroutineScript Coroutine;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo Info;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public KMSelectable[] Selectables;
    public Renderer[] ColoredObjects;
    public TPScript TP;
    public Transform Gear, Key;
    public Transform[] CylinderDisks;
    public TextMesh GearText;
    public TextMesh[] DisplayTexts, NixieTexts;
    public Texture[] ColorTextures;

    internal Init init;

    private void Awake()
    {
        Module.OnActivate += (init = new Init(Coroutine, this, TP)).Start;
	}
}
