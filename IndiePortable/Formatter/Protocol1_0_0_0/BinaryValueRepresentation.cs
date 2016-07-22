// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryValueRepresentation.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the BinaryValueRepresentation class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    internal class BinaryValueRepresentation
    {

        internal BinaryValueRepresentation(string name, byte[] value)
        {
            this.Name = name;
            this.Value = value;
        }


        internal string Name { get; private set; }


        internal byte[] Value { get; private set; }
    }
}
