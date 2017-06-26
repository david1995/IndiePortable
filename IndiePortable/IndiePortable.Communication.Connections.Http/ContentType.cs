// <copyright file="ContentType.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a MIME content type.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay("{FullName}")]
    public struct ContentType
        : IEquatable<ContentType>, IEquatable<string>
    {
        private static readonly Regex parser =
            new Regex(
                @"^([A-Za-z0-9-]+|\*)/(([A-Za-z-]+)\.)?([A-Za-z0-9-]+|\*)(\+([A-Za-z0-9-]+|\*))?(; (([A-Za-z0-9-]+)=([A-Za-z0-9*-]+|""[A-Za-z0-9-;]+""|\*)))?$");

        public ContentType(string source)
        {
            var match = parser.Match(source);
            if (!match.Success)
            {
                throw new ArgumentException("The specified string is not a valid MIME type string.", nameof(source));
            }

            this.TopLevelName = match.Groups[1].Value;
            this.TreeName = match.Groups[3].Value;
            this.SubtypeName = match.Groups[4].Value;
            this.Suffix = match.Groups[6].Value;
            this.ParameterKey = match.Groups[9].Value;
            this.ParameterValue = match.Groups[10].Value;

            var treeName = string.IsNullOrEmpty(this.TreeName) ? string.Empty : $"{this.TreeName}.";
            var suffix = string.IsNullOrEmpty(this.Suffix) ? string.Empty : $"+{this.Suffix}";
            var parameter = !string.IsNullOrEmpty(this.ParameterKey) && !string.IsNullOrEmpty(this.ParameterValue)
                          ? $"; {this.ParameterKey}={this.ParameterValue}"
                          : string.Empty;

            this.FullName = $"{this.TopLevelName}/{treeName}{this.SubtypeName}{suffix}{parameter}";
        }

        public ContentType(
            string topLevelName,
            string treeName,
            string subtypeName,
            string suffix = "",
            string parameterKey = "",
            string parameterValue = "")
            : this($"{topLevelName}/{(string.IsNullOrEmpty(treeName) ? string.Empty : $"{treeName}.")}{subtypeName}{(string.IsNullOrEmpty(suffix) ? string.Empty : $"+{suffix}")}{(!string.IsNullOrEmpty(parameterKey) && !string.IsNullOrEmpty(parameterValue) ? $"; {parameterKey}={parameterValue}" : string.Empty)}")
        {
        }

        public string TopLevelName { get; }

        public string TreeName { get; }

        public string SubtypeName { get; }

        public string Suffix { get; }

        public string ParameterKey { get; }

        public string ParameterValue { get; }

        public string FullName { get; }

        public static bool operator ==(ContentType value1, ContentType value2) => Equals(value1, value2);

        public static bool operator !=(ContentType value1, ContentType value2) => !(value1 == value2);

        public static bool Equals(ContentType value1, ContentType value2)
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

        /// <summary>
        /// Returns a <see cref="string" /> representing the <see cref="ContentType"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> representing the <see cref="ContentType"/>.
        /// </returns>
        public override string ToString() => this.FullName;

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is ContentType c
            && Equals(this, c);

        /// <inheritdoc/>
        public bool Equals(ContentType other) => Equals(this, other);

        /// <inheritdoc/>
        public bool Equals(string other)
            => IsContentTypeString(other)
            && Equals(this, new ContentType(other));
    }
}
