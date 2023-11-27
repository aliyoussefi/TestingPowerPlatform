using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Extensions
{
    public static class ArrayExtensions
    {
        //
        // Summary:
        //     Execute action for each element of the array
        //
        // Parameters:
        //   array:
        //
        //   action:
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength != 0)
            {
                ArrayTraverse arrayTraverse = new ArrayTraverse(array);
                do
                {
                    action(array, arrayTraverse.Position);
                }
                while (arrayTraverse.Step());
            }
        }
    }
}
