using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.WAD;
using static SabreTools.Models.WAD.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class WAD : BaseBinaryDeserializer<Models.WAD.File>
    {
        /// <inheritdoc/>
        public override Models.WAD.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life Texture Package to fill
            var file = new Models.WAD.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Lumps

            // Get the lump offset
            uint lumpOffset = header.LumpOffset;
            if (lumpOffset < 0 || lumpOffset >= data.Length)
                return null;

            // Seek to the lump offset
            data.Seek(lumpOffset, SeekOrigin.Begin);

            // Create the lump array
            file.Lumps = new Lump[header.LumpCount];
            for (int i = 0; i < header.LumpCount; i++)
            {
                var lump = ParseLump(data);
                if (lump == null)
                    return null;

                file.Lumps[i] = lump;
            }

            #endregion

            #region Lump Infos

            // Create the lump info array
            file.LumpInfos = new LumpInfo?[header.LumpCount];
            for (int i = 0; i < header.LumpCount; i++)
            {
                var lump = file.Lumps[i];
                if (lump == null)
                {
                    file.LumpInfos[i] = null;
                    continue;
                }

                if (lump.Compression != 0)
                {
                    file.LumpInfos[i] = null;
                    continue;
                }

                // Get the lump info offset
                uint lumpInfoOffset = lump.Offset;
                if (lumpInfoOffset < 0 || lumpInfoOffset >= data.Length)
                {
                    file.LumpInfos[i] = null;
                    continue;
                }

                // Seek to the lump info offset
                data.Seek(lumpInfoOffset, SeekOrigin.Begin);

                // Try to parse the lump info -- TODO: Do we ever set the mipmap level?
                var lumpInfo = ParseLumpInfo(data, lump.Type);
                file.LumpInfos[i] = lumpInfo;
            }

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();

            if (header == null)
                return null;
            if (header.Signature != SignatureString)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package lump
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package lump on success, null on error</returns>
        private static Lump? ParseLump(Stream data)
        {
            return data.ReadType<Lump>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package lump info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="type">Lump type</param>
        /// <param name="mipmap">Mipmap level</param>
        /// <returns>Filled Half-Life Texture Package lump info on success, null on error</returns>
        private static LumpInfo? ParseLumpInfo(Stream data, byte type, uint mipmap = 0)
        {
            // TODO: Use marshalling here instead of building
            LumpInfo lumpInfo = new LumpInfo();

            // Cache the initial offset
            long initialOffset = data.Position;

            // Type 0x42 has no name, type 0x43 does.  Are these flags?
            if (type == 0x42)
            {
                if (mipmap > 0)
                    return null;

                lumpInfo.Width = data.ReadUInt32();
                lumpInfo.Height = data.ReadUInt32();
                lumpInfo.PixelData = data.ReadBytes((int)(lumpInfo.Width * lumpInfo.Height));
                lumpInfo.PaletteSize = data.ReadUInt16();
            }
            else if (type == 0x43)
            {
                if (mipmap > 3)
                    return null;

                byte[]? name = data.ReadBytes(16);
                if (name != null)
                    lumpInfo.Name = Encoding.ASCII.GetString(name);
                lumpInfo.Width = data.ReadUInt32();
                lumpInfo.Height = data.ReadUInt32();
                lumpInfo.PixelOffset = data.ReadUInt32();
                lumpInfo.UnknownData = data.ReadBytes(12);

                // Cache the current offset
                long currentOffset = data.Position;

                // Seek to the pixel data
                data.Seek(initialOffset + lumpInfo.PixelOffset, SeekOrigin.Begin);

                // Read the pixel data
                lumpInfo.PixelData = data.ReadBytes((int)(lumpInfo.Width * lumpInfo.Height));

                // Seek back to the offset
                data.Seek(currentOffset, SeekOrigin.Begin);

                uint pixelSize = lumpInfo.Width * lumpInfo.Height;

                // Mipmap data -- TODO: How do we determine this during initial parsing?
                switch (mipmap)
                {
                    case 1: _ = data.ReadBytes((int)pixelSize); break;
                    case 2: _ = data.ReadBytes((int)(pixelSize + (pixelSize / 4))); break;
                    case 3: _ = data.ReadBytes((int)(pixelSize + (pixelSize / 4) + (pixelSize / 16))); break;
                    default: return null;
                }

                _ = data.ReadBytes((int)(pixelSize + (pixelSize / 4) + (pixelSize / 16) + (pixelSize / 64))); // Pixel data
                lumpInfo.PaletteSize = data.ReadUInt16();
                lumpInfo.PaletteData = data.ReadBytes((int)lumpInfo.PaletteSize * 3);
            }
            else
            {
                return null;
            }

            // Adjust based on mipmap level
            switch (mipmap)
            {
                case 1:
                    lumpInfo.Width /= 2;
                    lumpInfo.Height /= 2;
                    break;

                case 2:
                    lumpInfo.Width /= 4;
                    lumpInfo.Height /= 4;
                    break;

                case 3:
                    lumpInfo.Width /= 8;
                    lumpInfo.Height /= 8;
                    break;

                default:
                    return null;
            }

            return lumpInfo;
        }
    }
}