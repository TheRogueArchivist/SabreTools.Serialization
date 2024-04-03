using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class EverdriveSMDB : IFileSerializer<Models.EverdriveSMDB.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.EverdriveSMDB.MetadataFile? Deserialize(string? path)
        {
            var deserializer = new EverdriveSMDB();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.EverdriveSMDB.MetadataFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.EverdriveSMDB.Deserialize(stream);
        }
    }
}