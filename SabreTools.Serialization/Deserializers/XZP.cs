using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.XZP;
using static SabreTools.Models.XZP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class XZP : BaseBinaryDeserializer<Models.XZP.File>
    {
        /// <inheritdoc/>
        public override Models.XZP.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new XBox Package File to fill
            var file = new Models.XZP.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Directory Entries

            // Create the directory entry array
            file.DirectoryEntries = new DirectoryEntry[header.DirectoryEntryCount];

            // Try to parse the directory entries
            for (int i = 0; i < header.DirectoryEntryCount; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);
                if (directoryEntry == null)
                    return null;

                file.DirectoryEntries[i] = directoryEntry;
            }

            #endregion

            #region Preload Directory Entries

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory entry array
                file.PreloadDirectoryEntries = new DirectoryEntry[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory entries
                for (int i = 0; i < header.PreloadDirectoryEntryCount; i++)
                {
                    var directoryEntry = ParseDirectoryEntry(data);
                    if (directoryEntry == null)
                        return null;

                    file.PreloadDirectoryEntries[i] = directoryEntry;
                }
            }

            #endregion

            #region Preload Directory Mappings

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory mapping array
                file.PreloadDirectoryMappings = new DirectoryMapping[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory mappings
                for (int i = 0; i < header.PreloadDirectoryEntryCount; i++)
                {
                    var directoryMapping = ParseDirectoryMapping(data);
                    if (directoryMapping == null)
                        return null;

                    file.PreloadDirectoryMappings[i] = directoryMapping;
                }
            }

            #endregion

            #region Directory Items

            if (header.DirectoryItemCount > 0)
            {
                // Get the directory item offset
                uint directoryItemOffset = header.DirectoryItemOffset;
                if (directoryItemOffset < 0 || directoryItemOffset >= data.Length)
                    return null;

                // Seek to the directory items
                data.Seek(directoryItemOffset, SeekOrigin.Begin);

                // Create the directory item array
                file.DirectoryItems = new DirectoryItem[header.DirectoryItemCount];

                // Try to parse the directory items
                for (int i = 0; i < header.DirectoryItemCount; i++)
                {
                    var directoryItem = ParseDirectoryItem(data);
                    file.DirectoryItems[i] = directoryItem;
                }
            }

            #endregion

            #region Footer

            // Seek to the footer
            data.Seek(-8, SeekOrigin.End);

            // Try to parse the footer
            var footer = ParseFooter(data);
            if (footer == null)
                return null;

            // Set the package footer
            file.Footer = footer;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();

            if (header == null)
                return null;
            if (header.Signature != HeaderSignatureString)
                return null;
            if (header.Version != 6)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory entry on success, null on error</returns>
        private static DirectoryEntry? ParseDirectoryEntry(Stream data)
        {
            return data.ReadType<DirectoryEntry>();
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory mapping
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory mapping on success, null on error</returns>
        private static DirectoryMapping? ParseDirectoryMapping(Stream data)
        {
            return data.ReadType<DirectoryMapping>();
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory item on success, null on error</returns>
        private static DirectoryItem ParseDirectoryItem(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DirectoryItem directoryItem = new DirectoryItem();

            directoryItem.FileNameCRC = data.ReadUInt32();
            directoryItem.NameOffset = data.ReadUInt32();
            directoryItem.TimeCreated = data.ReadUInt32();

            // Cache the current offset
            long currentPosition = data.Position;

            // Seek to the name offset
            data.Seek(directoryItem.NameOffset, SeekOrigin.Begin);

            // Read the name
            directoryItem.Name = data.ReadNullTerminatedAnsiString();

            // Seek back to the right position
            data.Seek(currentPosition, SeekOrigin.Begin);

            return directoryItem;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File footer on success, null on error</returns>
        private static Footer? ParseFooter(Stream data)
        {
            var footer = data.ReadType<Footer>();

            if (footer == null)
                return null;
            if (footer.Signature != FooterSignatureString)
                return null;

            return footer;
        }
    }
}