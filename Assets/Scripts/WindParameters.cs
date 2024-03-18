public class WindParameters
{
    public float windForceMin;
    public float windForceMax;
    public float windDirectionChangeDelta;

    public WindParameters(float windForceMin, float windForceMax, float windDirectionChangeDelta)
    {
        this.windForceMin = windForceMin;
        this.windForceMax = windForceMax;
        this.windDirectionChangeDelta = windDirectionChangeDelta;
    }
}
