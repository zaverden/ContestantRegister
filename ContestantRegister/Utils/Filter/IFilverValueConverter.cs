using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Utils.Filter
{
    public interface IFilverValueConverter
    {
        object Convert(object value);
    }
}
