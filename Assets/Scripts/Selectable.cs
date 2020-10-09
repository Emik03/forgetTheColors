using System.Diagnostics;

public class Selectable 
{
    public Selectable(FTCScript FTC, Init init, Render render)
    {
        this.FTC = FTC;
        this.init = init;
        this.render = render;
    }

    private readonly FTCScript FTC;
    private readonly Init init;
    private readonly Render render;

    internal KMSelectable.OnInteractHandler Interact(byte index)
    {
        return delegate ()
        {
            UnityEngine.Debug.Log(index);
            switch (index)
            {
                case 0:
                    break;

                case 1:
                    break;

                case 2:
                    render.AssignRandom(init.legacy);
                    break;
            }

            return false;
        };
    }
}
