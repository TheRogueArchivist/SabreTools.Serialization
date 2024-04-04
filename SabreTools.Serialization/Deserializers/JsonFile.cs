﻿using System.IO;
using System.Text;
using Newtonsoft.Json;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonFile<T> :
        IFileDeserializer<T>,
        IStreamDeserializer<T>
    {
        #region IFileDeserializer

        /// <inheritdoc/>
        public virtual T? Deserialize(string? path)
            => Deserialize(path, new UTF8Encoding(false));

        /// <summary>
        /// Deserialize a file into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="path">Path to deserialize from</param>
        /// <param name="encoding">Encoding to parse text as</param>
        /// <returns>Filled object on success, null on error</returns>
        public T? Deserialize(string? path, Encoding encoding)
        {
            using var data = PathProcessor.OpenStream(path);
            return Deserialize(data, encoding);
        }

        #endregion
    
        #region IStreamDeserializer

        /// <inheritdoc/>
        public virtual T? Deserialize(Stream? data)
            => Deserialize(data, new UTF8Encoding(false));

        /// <summary>
        /// Deserialize a Stream into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Stream to parse</param>
        /// <param name="encoding">Text encoding to use</param>
        /// <returns>Filled object on success, null on error</returns>
        public T? Deserialize(Stream? data, Encoding encoding)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the serializer and the reader
            var serializer = JsonSerializer.Create();
            var streamReader = new StreamReader(data, encoding);
            var jsonReader = new JsonTextReader(streamReader);

            // Perform the deserialization and return
            return serializer.Deserialize<T>(jsonReader);
        }

        #endregion
    }
}