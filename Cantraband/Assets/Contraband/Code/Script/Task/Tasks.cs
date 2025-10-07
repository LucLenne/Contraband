using System.Threading.Tasks;

public static class Tasks
{
    public static async Task WaitSeconds(int seconds)
    {
        await Task.Delay(seconds * 1000);
    }
}
