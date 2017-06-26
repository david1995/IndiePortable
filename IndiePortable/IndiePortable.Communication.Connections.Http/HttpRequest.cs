// <copyright file="HttpRequest.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class HttpRequest
    {

        public HttpRequest(HttpMethod method, Uri requestUri, string httpVersion)
        {
            this.Method = method;
            this.RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
            this.HttpVersion = string.IsNullOrEmpty(httpVersion)
                             ? throw new ArgumentNullException(nameof(httpVersion))
                             : httpVersion;
        }

        public HttpHeaderCollection Headers { get; } = new HttpHeaderCollection();

        public HttpMethod Method { get; }

        public Uri RequestUri { get; }

        public string HttpVersion { get; }

        public IList<ContentType> Accept { get; } = new List<ContentType>();

        public byte[] Body { get; set; }

        public string AcceptCharset
        {
            get => this.Headers["Accept-Charset"] as string;
            set => this.Headers["Accept-Charset"] = value;
        }

        public Encoding AcceptEncoding { get; set; }

        public string AcceptLanguage { get; set; }

        public DateTime? AcceptDatetime
        {
            get => this.Headers["Accept-Datetime"] as DateTime?;
            set => this.Headers["Accept-Datetime"] = value;
        }

        public string Authorization
        {
            get => this.Headers["Authorization"] as string;
            set => this.Headers["Authorization"] = value;
        }

        public string Connection
        {
            get => this.Headers["Connection"] as string;
            set => this.Headers["Connection"] = value;
        }

        public string Cookie
        {
            get => this.Headers["Cookie"] as string;
            set => this.Headers["Cookie"] = value;
        }

        public long? ContentLength
        {
            get => this.Headers["Content-Length"] as long?;
            set => this.Headers["Content-Length"] = value;
        }

        public IList<ContentType> ContentType
        {
            get
            {
                if (this.Headers["Content-Type"] is null)
                {
                    this.Headers["Content-Type"] = new List<ContentType>();
                }

                return this.Headers["Content-Type"] as IList<ContentType>;
            }
        }

        public DateTime? Date
        {
            get => this.Headers["Date"] as DateTime?;
            set => this.Headers["Date"] = value;
        }

        public string Expect
        {
            get => this.Headers["Expect"] as string;
            set => this.Headers["Expect"] = value;
        }

        public string Forwarded
        {
            get => this.Headers["Forwarded"] as string;
            set => this.Headers["Forwarded"] = value;
        }

        public string From
        {
            get => this.Headers["From"] as string;
            set => this.Headers["From"] = value;
        }

        public string Host
        {
            get => this.Headers["Host"] as string;
            set => this.Headers["Host"] = value;
        }

        public Uri Origin
        {
            get => this.Headers["Origin"] as Uri;
            set => this.Headers["Origin"] = value;
        }

        public string ProxyAuthentication
        {
            get => this.Headers["Proxy-Authentication"] as string;
            set => this.Headers["Proxy-Authentication"] = value;
        }

        public string Range
        {
            get => this.Headers["Range"] as string;
            set => this.Headers["Range"] = value;
        }

        public Uri Referer
        {
            get => this.Headers["Referer"] as Uri;
            set => this.Headers["Referer"] = value;
        }

        public string Upgrade
        {
            get => this.Headers["Upgrade"] as string;
            set => this.Headers["Upgrade"] = value;
        }

        public string UserAgent
        {
            get => this.Headers["User-Agent"] as string;
            set => this.Headers["User-Agent"] = value;
        }
    }
}
