﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Extensions
{
    internal class ArrayTraverse
    {
        public int[] Position;

        private int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }

            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; i++)
            {
                if (Position[i] < maxLengths[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
