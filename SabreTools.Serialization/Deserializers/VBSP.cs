using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.VBSP;
using static SabreTools.Models.VBSP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class VBSP : BaseBinaryDeserializer<Models.VBSP.File>
    {
        /// <inheritdoc/>
        public override Models.VBSP.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life 2 Level to fill
            var file = new Models.VBSP.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life 2 Level header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var header = new Header();

            byte[]? signature = data.ReadBytes(4);
            if (signature == null)
                return null;

            header.Signature = Encoding.ASCII.GetString(signature);
            if (header.Signature != SignatureString)
                return null;

            header.Version = data.ReadInt32();
            if ((header.Version < 19 || header.Version > 22) && header.Version != 0x00040014)
                return null;

            header.Lumps = new Lump[HL_VBSP_LUMP_COUNT];
            for (int i = 0; i < HL_VBSP_LUMP_COUNT; i++)
            {
                var lump = ParseLump(data, header.Version);
                if (lump == null)
                    return null;

                header.Lumps[i] = lump;
            }

            header.MapRevision = data.ReadInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level lump
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="version">VBSP version</param>
        /// <returns>Filled Half-Life 2 Level lump on success, null on error</returns>
        private static Lump? ParseLump(Stream data, int version)
        {
            return data.ReadType<Lump>();

            // This block was commented out because test VBSPs with header
            // version 21 had the values in the "right" order already and
            // were causing decompression issues

            //if (version >= 21 && version != 0x00040014)
            //{
            //    uint temp = lump.Version;
            //    lump.Version = lump.Offset;
            //    lump.Offset = lump.Length;
            //    lump.Length = temp;
            //}
            //
            //return lump
        }
    }
}