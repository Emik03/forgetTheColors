using ForgetTheColors;
using UnityEngine;

/// <summary>
/// Forget The Colors - A Keep Talking and Nobody Explodes Modded Module by Emik and Cooldoom.
/// </summary>
public class FTCScript : MonoBehaviour
{
    public CoroutineScript Coroutine;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBossModule Boss;
    public KMBombInfo Info;
    public KMColorblindMode Colorblind;
    public KMRuleSeedable Rule;
    public KMSelectable[] Selectables;
    public LegacyFTC Legacy;
    public Renderer[] ColoredObjects;
    public TPScript TP;
    public Transform Gear, Key;
    public Transform[] CylinderDisks;
    public TextMesh GearText, DebugText;
    public TextMesh[] BackingTexts, DisplayTexts, NixieTexts;
    public Texture[] ColorTextures;

    internal Init init;

    private void Awake()
    {
        Module.OnActivate += (init = new Init(Coroutine, Legacy, this, TP)).Start;
	}
}
