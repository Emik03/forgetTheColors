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
            switch (index)
            {
                case 0:
                    break;

                case 1:
                    break;

                case 2:
                    render.AssignRandom(init.legacy);
                    calculate.Current();
                    break;
            }

            return false;
        };
    }
}
