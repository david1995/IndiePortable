// <copyright file="ContentTypes.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    public static class ContentTypes
    {
        /// <summary>
        /// Gets the application/json content-type.
        /// </summary>
        /// <value>
        /// The application/json content-type.
        /// </value>
        public static ContentType ApplicationJson { get; } = new ContentType("application/json");

        /// <summary>
        /// Gets the application/PDF content-type.
        /// </summary>
        /// <value>
        /// The application/PDF content-type.
        /// </value>
        public static ContentType ApplicationPdf { get; } = new ContentType("application/pdf");

        /// <summary>
        /// Gets the text/HTML content-type.
        /// </summary>
        /// <value>
        /// The text/HTML content-type.
        /// </value>
        public static ContentType TextHtml { get; } = new ContentType("text/html");

        /// <summary>
        /// Gets the text/plain content-type.
        /// </summary>
        /// <value>
        /// The text/plain content-type.
        /// </value>
        public static ContentType TextPlain { get; } = new ContentType("text/plain");
    }
}
