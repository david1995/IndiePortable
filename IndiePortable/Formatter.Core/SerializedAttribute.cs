// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedAttribute.cs" company="David Eiwen">
// Copyright © 2017 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializedAttribute class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializedAttribute
        : Attribute
    {

        public string SerializedName { get; set; }


        public int ObfuscationIndex { get; set; }
    }
}
