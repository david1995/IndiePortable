// <copyright file="MimeType.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Http
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a MIME type.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay("{FullName}")]
    public struct MimeType
        : IEquatable<MimeType>, IEquatable<string>
    {
        private static readonly Regex parser =
            new Regex(
                @"^([A-Za-z0-9-]+|\*)/(([A-Za-z-]+)\.)?([A-Za-z0-9-]+|\*)(\+([A-Za-z0-9-]+|\*))?(; (([A-Za-z0-9-]+)=([A-Za-z0-9*-]+|""[A-Za-z0-9-;]+""|\*)))?$");

        public MimeType(
            string topLevelName,
            string subtypeName,
            string treeName = "",
            string suffix = "",
            string parameterKey = "",
            string parameterValue = "")
        {
            this.TopLevelName = string.IsNullOrEmpty(topLevelName)
                              ? throw new ArgumentNullException(nameof(topLevelName))
                              : topLevelName;
            this.SubtypeName = string.IsNullOrEmpty(subtypeName)
                             ? throw new ArgumentNullException(nameof(subtypeName))
                             : subtypeName;

            this.TreeName = treeName;
            this.Suffix = suffix;
            this.ParameterKey = string.IsNullOrEmpty(parameterValue)
                              ? throw new ArgumentNullException(nameof(parameterValue))
                              : parameterKey;
            this.ParameterValue = string.IsNullOrEmpty(parameterKey)
                                ? throw new ArgumentNullException(nameof(parameterKey))
                                : parameterValue;
        }

        public string TopLevelName { get; }

        public string TreeName { get; }

        public string SubtypeName { get; }

        public string Suffix { get; }

        public string ParameterKey { get; }

        public string ParameterValue { get; }

        public string FullName
            => $"{this.TopLevelName}/{(string.IsNullOrEmpty(this.TreeName) ? string.Empty : $"{this.TreeName}.")}{this.SubtypeName}{(string.IsNullOrEmpty(this.Suffix) ? string.Empty : $"+{this.Suffix}")}{(!string.IsNullOrEmpty(this.ParameterKey) && !string.IsNullOrEmpty(this.ParameterValue) ? $"; {this.ParameterKey}={this.ParameterValue}" : string.Empty)}";

        public static bool operator ==(MimeType value1, MimeType value2) => Equals(value1, value2);

        public static bool operator !=(MimeType value1, MimeType value2) => !(value1 == value2);

        /// <summary>
        /// Determines whether two <see cref="MimeType"/> values can be considered equal.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>A value indicating whether <paramref name="value1"/> and <paramref name="value2"/> can be considered equal.</returns>
        public static bool Equals(MimeType value1, MimeType value2)
            => value1.TopLevelName?.ToUpper() == value2.TopLevelName?.ToUpper()
            && value1.TreeName?.ToUpper() == value2.TreeName?.ToUpper()
            && value1.SubtypeName?.ToUpper() == value2.SubtypeName?.ToUpper()
            && value1.Suffix?.ToUpper() == value2.Suffix?.ToUpper()
            && value1.ParameterKey?.ToUpper() == value2.ParameterKey?.ToUpper()
            && value1.ParameterValue?.ToUpper() == value2.ParameterValue?.ToUpper();

        /// <summary>
        /// Determines whether the specified value is a MIME content-type string.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is a MIME content-type string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsContentTypeString(string value) => parser.IsMatch(value);

        public static MimeType Parse(string source)
        {
            var match = parser.Match(source);
            if (!match.Success)
            {
                throw new ArgumentException("The specified string is not a valid MIME type string.", nameof(source));
            }

            var topLevelName = match.Groups[1].Value;
            var treeName = match.Groups[3].Value;
            var subtypeName = match.Groups[4].Value;
            var suffix = match.Groups[6].Value;
            var parameterKey = match.Groups[9].Value;
            var parameterValue = match.Groups[10].Value;

            return new MimeType(topLevelName, treeName, subtypeName, suffix, parameterKey, parameterValue);
        }

        public static bool TryParse(string source, out MimeType result)
        {
            var match = parser.Match(source);
            if (match.Success)
            {
                var topLevelName = match.Groups[1].Value;
                var treeName = match.Groups[3].Value;
                var subtypeName = match.Groups[4].Value;
                var suffix = match.Groups[6].Value;
                var parameterKey = match.Groups[9].Value;
                var parameterValue = match.Groups[10].Value;

                result = new MimeType(topLevelName, treeName, subtypeName, suffix, parameterKey, parameterValue);
                return true;
            }

            result = default(MimeType);
            return false;
        }

        public static (bool Success, MimeType Result) TryParse(string source) => (TryParse(source, out var result), result);

        /// <summary>
        /// Returns a <see cref="string" /> representing the <see cref="MimeType"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> representing the <see cref="MimeType"/>.
        /// </returns>
        public override string ToString() => this.FullName;

        /// <inheritdoc/>
        public override int GetHashCode() => this.FullName.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is MimeType c
            && Equals(this, c);

        /// <inheritdoc/>
        public bool Equals(MimeType other) => Equals(this, other);

        /// <inheritdoc/>
        public bool Equals(string other) => TryParse(other, out var o) && Equals(this, o);
    }
}
