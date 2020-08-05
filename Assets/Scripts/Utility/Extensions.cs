using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Nito.AsyncEx;
using UnityAsync;
using System.Threading;

namespace Utility
{
    public static class Extensions
    {
        private static List<Type> NumericTypes = new List<Type>() { typeof(int), typeof(float), typeof(double) };

        public static bool IsNumericType(this Type type)
        {
            if(NumericTypes.Contains(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    } 
}
