using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class IRD : IFileSerializer<Models.IRD.IRD>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(Models.IRD.IRD obj, string path)
#else
        public bool Serialize(Models.IRD.IRD? obj, string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.IRD().Serialize(obj))
            {
                if (stream == null)
                    return false;

                using (var fs = System.IO.File.OpenWrite(path))
                {
                    stream.CopyTo(fs);
                    return true;
                }
            }
        }
    }
}