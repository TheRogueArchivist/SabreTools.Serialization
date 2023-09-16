using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class Quantum : WrapperBase<Models.Quantum.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Quantum Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public Quantum(Models.Quantum.Archive model, byte[] data, int offset)
#else
        public Quantum(Models.Quantum.Archive? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public Quantum(Models.Quantum.Archive model, Stream data)
#else
        public Quantum(Models.Quantum.Archive? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
#if NET48
        public static Quantum Create(byte[] data, int offset)
#else
        public static Quantum? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
#if NET48
        public static Quantum Create(Stream data)
#else
        public static Quantum? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new Streams.Quantum().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new Quantum(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}