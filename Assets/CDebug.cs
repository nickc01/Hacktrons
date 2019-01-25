//#define DEBUGGING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




public static class CDebug
{
	public static void Log(object message)
    {
#if DEBUGGING
        Debug.Log(message);
#endif
    }
    public static void Log(object message,UnityEngine.Object context)
    {
#if DEBUGGING
        Debug.Log(message,context);
#endif
    }
}