public class Legacy
{
    public Legacy(CoroutineScript coroutine, FTCScript FTC, Init init)
    {
        this.coroutine = coroutine;
        this.FTC = FTC;
        this.init = init;

        calculate = init.calculate;
        render = init.render;
    }

    private readonly Calculate calculate;
    private readonly CoroutineScript coroutine;
    private readonly FTCScript FTC;
    private readonly Init init;
    private readonly Render render;

    internal void Start()
    {
        init.ResetArrays();

        render.Assign(displays: new[] { 420, 69 }, 
                      gears: new[] { 5, 5 }, 
                      nixies: new[] { 5, 5 }, 
                      cylinders: new[] { 4, 5, 6 });

        calculate.Current();
    }
}
