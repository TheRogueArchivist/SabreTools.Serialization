using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Bytes
{
    public partial class XZP : IByteSerializer<Models.XZP.File>
    {
        /// <inheritdoc cref="IByteSerializer.Deserialize(byte[]?, int)"/>
        public static Models.XZP.File? Deserialize(byte[]? data, int offset)
        {
            var obj = new XZP();
            return obj.DeserializeImpl(data, offset);
        }

        /// <inheritdoc/>
        public Models.XZP.File? DeserializeImpl(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return new Streams.XZP().Deserialize(dataStream);
        }
    }
}