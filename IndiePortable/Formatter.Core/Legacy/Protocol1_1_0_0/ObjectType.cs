// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectType.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectType enum.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public enum ObjectType
        : byte
    {

        RefType = 0x00,


        Primitive = 0x01,


        Enum = 0x02,


        ValueType = 0x03,


        String = 0x04,


        Null = 0x05
    }
}
