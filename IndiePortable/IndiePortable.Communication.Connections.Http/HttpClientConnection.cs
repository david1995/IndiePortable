// <copyright file="HttpClientConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Devices.Connections.Decorators;
    using IndiePortable.AdvancedTasks;
    using IndiePortable.Collections.Linq;
    using IndiePortable.Communication.Core.Devices.Connections;

    public class HttpClientConnection
        : AsymmetricConnectionDecorator<HttpRequest, HttpResponse, IPEndPoint>
    {

        private static IReadOnlyDictionary<HttpMethod, string> MethodToString
            = new Dictionary<HttpMethod, string>
            {
                [HttpMethod.Get] = "GET",
                [HttpMethod.Post] = "POST",
                [HttpMethod.Put] = "PUT",
                [HttpMethod.Update] = "UPDATE",
                [HttpMethod.Delete] = "DELETE",
                [HttpMethod.Head] = "HEAD",
                [HttpMethod.Trace] = "TRACE",
                [HttpMethod.Options] = "OPTIONS",
                [HttpMethod.Connect] = "CONNECT",
                [HttpMethod.Patch] = "PATCH",
            };

        private static IReadOnlyDictionary<string, HttpMethod> StringToMethod
            = new Dictionary<string, HttpMethod>
            {
                ["GET"] = HttpMethod.Get,
                ["POST"] = HttpMethod.Post,
                ["PUT"] = HttpMethod.Put,
                ["UPDATE"] = HttpMethod.Update,
                ["DELETE"] = HttpMethod.Delete,
                ["HEAD"] = HttpMethod.Head,
                ["TRACE"] = HttpMethod.Trace,
                ["OPTIONS"] = HttpMethod.Options,
                ["CONNECT"] = HttpMethod.Connect,
                ["PATCH"] = HttpMethod.Patch,
            };

        public HttpClientConnection(StreamConnectionBase<IPEndPoint> decoratedConnection)
            : base(decoratedConnection)
        {
        }

        public override IPEndPoint RemoteAddress { get; }

        protected override void ActivateOverride() => throw new NotImplementedException();

        protected override void DisconnectOverride() => throw new NotImplementedException();

        protected override Task DisconnectAsyncOverride() => throw new NotImplementedException();

        protected override void SendOverride(HttpRequest message)
        {
            var msg = DownstreamConverter(message);
            this.DecoratedConnection.DataStream.Write(msg, 0, msg.Length);
        }

        protected override async Task SendAsyncOverride(HttpRequest message)
        {
            var msg = DownstreamConverter(message);
            await this.DecoratedConnection.DataStream.WriteAsync(msg, 0, msg.Length);
        }

        private static byte[] DownstreamConverter(HttpRequest request)
        {
            using (var memstr = new MemoryStream())
            {
                using (var sw = new StreamWriter(memstr, Encoding.ASCII) { NewLine = "\r\n" })
                {
                    // GET / HTTP/1.1
                    sw.WriteLine($"{MethodToString[request.Method]} {request.RequestUri} {request.HttpVersion}");

                    // Field: Value
                    request.Headers.Where(f => f.HasValue)
                                   .ForEach(f => sw.WriteLine($"{f.Name}: {f.Value}"));

                    sw.WriteLine();
                    sw.Flush();

                    if (request.Body is byte[] b)
                    {
                        memstr.Write(b, 0, b.Length);
                    }

                    return memstr.ToArray();
                }
            }
        }

        private async void Listener(ITaskConnection conn)
        {
            async Task<string> ReadLineAsync(CancellationTokenSource cts)
            {
                var cr = false;
                var lf = false;

                var lineBuilder = new StringBuilder(64);
                var buffer = new byte[64];
                var bufferPos = 0;

                while (!cr && !lf)
                {
                    var read = await this.DecoratedConnection.DataStream.ReadAsync(buffer, bufferPos, 1, cts.Token);
                    if (read == 1)
                    {
                        lf = buffer[bufferPos] == (byte)'\n';
                        cr = buffer[bufferPos] == (byte)'\r' || (bufferPos > 0 && buffer[bufferPos - 1] == (byte)'\r' && lf);

                        if (bufferPos == buffer.Length || (cr && lf))
                        {
                            var str = Encoding.ASCII.GetString(buffer, 0, cr && lf ? bufferPos - 2 : bufferPos);
                            lineBuilder.Append(str);
                            bufferPos = 0;
                        }
                        else
                        {
                            bufferPos++;
                        }
                    }
                    else if (cts.IsCancellationRequested)
                    {
                        throw new IOException();
                    }
                }

                return lineBuilder.ToString();
            }

            try
            {
                while (!conn.MustFinish)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        // TODO: check whether cts.Cancel() throws exception
                        void OnStopRequested(object s, EventArgs e) => cts.Cancel(false);

                        conn.StopRequested += OnStopRequested;

                        if (!conn.MustFinish)
                        {
                            try
                            {
                                var header = await ReadLineAsync(cts);
                                var headerSplit = header.Split(' ');
                                var method = StringToMethod[headerSplit[0]];
                                var uri = new Uri(headerSplit[1]);
                                var httpVersion = headerSplit[2];

                                var rq = new HttpRequest(method, uri, httpVersion);

                                string field;
                                do
                                {
                                    field = await ReadLineAsync(cts);
                                    if (!string.IsNullOrEmpty(field))
                                    {
                                        var key = string.Join(string.Empty, field.SkipWhile(c => c != ':')).Trim();
                                        var value = field.Substring(field.IndexOf(':') + 1).Trim();
                                        rq.Headers[key] = value;
                                    }
                                }
                                while (!string.IsNullOrEmpty(field));
                            }
                            catch (IOException)
                            {
                            }
                        }

                        conn.StopRequested -= OnStopRequested;
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception exc)
            {
                conn.ThrowException(exc);
            }

            conn.Return();
        }
    }
}
