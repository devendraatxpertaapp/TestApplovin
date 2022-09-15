using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CustomWaiting
{

    public static async Task _Waiter(int m_mili_second_to_wait)
    {
        await Task.Delay(m_mili_second_to_wait);
    }
}
