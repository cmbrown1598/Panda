using System.Collections;
using System.Collections.Generic;

namespace Panda
{
    public interface IOutputMap
    {
        IList<IOutputRule> Rules { get; }
    }
}

