using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Common
{
    public static class Extender
    {
        public static TData Clone<TData>(this TData o)
        {
            Type t = o.GetType();
            TData result = (TData)t.GetConstructor(new Type[0]).Invoke(new object[0]);
            foreach(var p in t.GetProperties())
                if(p.CanRead && p.CanWrite)
                    p.SetValue(result, p.GetValue(o));
            return result;
        }
    }
}
