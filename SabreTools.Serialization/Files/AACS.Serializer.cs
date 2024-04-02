using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc/>
        public bool Serialize(Models.AACS.MediaKeyBlock? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.AACS().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}