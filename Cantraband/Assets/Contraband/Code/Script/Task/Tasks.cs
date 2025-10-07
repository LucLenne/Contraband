using UnityEngine;
using System.Threading.Tasks;

public class Tasks : MonoBehaviour
{
    private async Task WaitSeconds(int seconds)
    {
        IsBetweenTransactions = true;
        await Task.Delay(seconds * 1000);
        IsBetweenTransactions = false;
    }
}
