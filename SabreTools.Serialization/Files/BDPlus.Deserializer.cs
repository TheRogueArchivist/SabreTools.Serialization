using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BDPlus : IFileSerializer<Models.BDPlus.SVM>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BDPlus.SVM? Deserialize(string? path)
        {
            var obj = new BDPlus();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.BDPlus.SVM? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BDPlus().Deserialize(stream);
        }
    }
}