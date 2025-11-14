using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sandbox.Project.Scripts.Helpers.BetterSpaceStringHelper
{
    public static class StringHelper
    {
        private static readonly Regex SWhitespace = new(@"\s+");
        private static readonly Regex SDots = new(@"\.+");


        public static string ReplaceUnderscores(string input)
        {
            try
            {
                return input.Replace("_", " ");
            }
            catch (Exception e)
            {
                ShowErrorLog(e);
                return string.Empty;
            }
        }

        public static string ReplaceWhitespace(string input, string replacement)
        {
            try
            {
                return SWhitespace.Replace(input, replacement);
            }
            catch (Exception e)
            {
                ShowErrorLog(e);
                return string.Empty;
            }
        }

        public static string ReplaceDots(string input, string replacement)
        {
            try
            {
                return SDots.Replace(input, replacement);
            }
            catch (Exception e)
            {
                ShowErrorLog(e);
                return string.Empty;
            }
        }

        private static void ShowErrorLog(Exception e) => Debug.LogError($"StringHelper ERROR: {e.Message}");
        
        /// <summary>
        /// Remove prefix "Chunk_N" and ".asset".
        /// </summary>
        public static string RemoveChunkPrefix(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            const string pattern = @"^Chunk_\d+\/";

            string withoutPrefix = Regex.Replace(input, pattern, string.Empty);
            return Path.GetFileNameWithoutExtension(withoutPrefix);
        }       
        
        public static string RemoveChunkPrefixes(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            const string pattern = @"^(?:Chunk_\d+\/)+";

            string withoutPrefix = Regex.Replace(input, pattern, string.Empty);
            return Path.GetFileNameWithoutExtension(withoutPrefix);
        }        
        
        public static string CleanAssetName(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            
            string withoutPrefixes = Regex.Replace(input, @"^(?:Chunk_\d+\/)+", string.Empty);
            string fileName = Path.GetFileName(withoutPrefixes);
            
            fileName = Regex.Replace(fileName, @"\s*\(Clone\)$", string.Empty);
            fileName = Regex.Replace(fileName, @"\.asset$", string.Empty, RegexOptions.IgnoreCase);

            return fileName;
        }
    }
}