using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.Testing.Helpers
{
    // This defaults to Inherited = true.
    public class MyAttribute : Attribute
    {
        //...
    }

    public class MyCategoryAttribute : TestCategoryBaseAttribute
    {
        private string myvalue;
        public MyCategoryAttribute(string myvalue)
        {
            this.testCategories = new List<string>();
            this.testCategories.Add(myvalue);
        }

        private IList<string> testCategories;

        //
        // Summary:
        //     Gets the test categories that has been applied to the test.
        public override IList<string> TestCategories => testCategories;
        //...
    }
}
