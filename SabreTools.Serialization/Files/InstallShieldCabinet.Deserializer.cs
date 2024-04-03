using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class InstallShieldCabinet : IFileSerializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.InstallShieldCabinet.Cabinet? Deserialize(string? path)
        {
            var obj = new InstallShieldCabinet();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.InstallShieldCabinet.Cabinet? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.InstallShieldCabinet().Deserialize(stream);
        }
    }
}