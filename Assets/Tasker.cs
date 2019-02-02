using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Tasker
{
    public static Task Run(Action action)
    {
        return Task.Run(() => {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError("Error in task : " + e);
            }
        });
    }
}
