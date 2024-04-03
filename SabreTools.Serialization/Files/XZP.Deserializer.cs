using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XZP : IFileSerializer<Models.XZP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.XZP.File? Deserialize(string? path)
        {
            var obj = new XZP();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.XZP.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.XZP().Deserialize(stream);
        }
    }
}