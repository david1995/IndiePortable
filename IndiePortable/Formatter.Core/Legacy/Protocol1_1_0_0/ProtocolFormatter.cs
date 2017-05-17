// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolFormatter.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ProtocolFormatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a binary formatter for the version 1.1.
    /// </summary>
    /// <seealso cref="IProtocolFormatter" />
    public class ProtocolFormatter
        : IProtocolFormatter
    {

        public Version ProtocolVersion { get; } = new Version(1, 1, 0, 0);


        public object Deserialize(Stream source, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            throw new NotImplementedException();


            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (surrogateSelectors == null)
            {
                throw new ArgumentNullException(nameof(surrogateSelectors));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            var header = HeaderInformation.FromStream(source);

            
        }


        public void Serialize(Stream target, object graph, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            if (object.ReferenceEquals(target, null))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (object.ReferenceEquals(surrogateSelectors, null))
            {
                throw new ArgumentNullException(nameof(surrogateSelectors));
            }

            throw new NotImplementedException();
        }
    }
}
