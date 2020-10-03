using JetBrains.Annotations;
using System;

namespace Coimbra.BuildManagement.Local
{
    [Flags]
    internal enum OpenBuiltPlayerOptions
    {
        [UsedImplicitly] DontOpen = 0,
        [UsedImplicitly] OpenOriginalOutput = 1,
        [UsedImplicitly] OpenStandardizedOutput = 2,
        [UsedImplicitly] OpenBoth = ~0,
    }
}
