using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IFrequentFlyerValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
    }
}
