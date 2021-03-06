﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MusicallyApi.Cache.Handlers
{
    /// <summary>
    ///     Caches API stuff to the disk.
    /// </summary>
    public class FileCacheHandler : ICacheHandler
    {
        private readonly string _cacheDirectory;

        /// <summary>
        ///     Caches API stuff to the disk.
        /// </summary>
        /// <param name="cacheDirectory">The directory where cache files will be stored.</param>
        public FileCacheHandler(string cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;

            Directory.CreateDirectory(_cacheDirectory);
        }

        /// <inheritdoc />
        public MusicallyCache Load(string email)
        {
            var fileName = GetFileName(email);

            if (!File.Exists(fileName))
            {
                return new MusicallyCache();
            }

            return JsonConvert.DeserializeObject<MusicallyCache>(File.ReadAllText(fileName));
        }

        /// <inheritdoc />
        public void Save(string email, MusicallyCache cache)
        {
            var fileName = GetFileName(email);

            File.WriteAllText(fileName, JsonConvert.SerializeObject(cache, Formatting.Indented));
        }

        /// <inheritdoc />
        public void Delete(string email)
        {
            var fileName = GetFileName(email);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private string GetFileName(string email)
        {
            var hash = Hash(email);
            var fileName = $"{hash}.json";

            return Path.Combine(_cacheDirectory, fileName);
        }

        private static string Hash(string input)
        {
            using (var hash = new SHA1CryptoServiceProvider())
            {
                var bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(input));

                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            }
        }
    }
}
