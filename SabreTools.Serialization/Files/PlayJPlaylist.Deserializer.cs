using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJPlaylist : IFileSerializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PlayJ.Playlist? Deserialize(string? path)
        {
            var obj = new PlayJPlaylist();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PlayJ.Playlist? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PlayJPlaylist().Deserialize(stream);
        }
    }
}