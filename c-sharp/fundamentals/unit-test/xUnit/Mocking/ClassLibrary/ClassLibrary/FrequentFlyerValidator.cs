using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class FrequentFlyerValidator : IFrequentFlyerValidator
    {
        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }
    }
}
