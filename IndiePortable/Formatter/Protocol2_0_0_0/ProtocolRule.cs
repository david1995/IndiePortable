// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolRule.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ProtocolRule class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol2_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public abstract class ProtocolRule<T>
    {

        protected ProtocolRule()
        {
        }


        public abstract bool Check(FieldData target, FormatterState state);


        public abstract T Transform(FieldData target, FormatterState state);
    }
}
