using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.AACS.MediaKeyBlock? Deserialize(string? path)
        {
            var obj = new AACS();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.AACS().Deserialize(stream);
        }
    }
}