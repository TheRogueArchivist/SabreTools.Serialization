using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class BSP : IStreamSerializer<Models.BSP.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.BSP.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.BSP.File? obj) => throw new NotImplementedException();
#endif
    }
}