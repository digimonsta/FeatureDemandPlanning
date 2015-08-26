﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureDemandPlanning.Comparers
{
    public class StringArrayIndexComparer : IComparer<string[]>
    {
        public StringArrayIndexComparer(int sortIndex, bool descending)
        {
            _sortIndex = sortIndex;
            _descending = descending;
        }

        public int Compare(string[] x, string[] y)
        {
            if (_descending)
            {
                return y[_sortIndex].CompareTo(x[_sortIndex]);
            }
            else
            {
                return x[_sortIndex].CompareTo(y[_sortIndex]);
            }
        }

        private int _sortIndex = 0;
        private bool _descending = false;
    }
}
