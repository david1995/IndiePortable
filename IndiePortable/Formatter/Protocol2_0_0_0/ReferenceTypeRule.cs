// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceTypeRule.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ReferenceTypeRule class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol2_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;



    public class ReferenceTypeRule<T>
        : ProtocolRule<T>
    {

        private readonly Func<FieldData, FormatterState, T> transform;


        public ReferenceTypeRule(Func<FieldData, FormatterState, T> transform)
        {
            if (object.ReferenceEquals(transform, null))
            {
                throw new ArgumentNullException(nameof(transform));
            }

            this.transform = transform;
        }

        public override bool Check(FieldData target, FormatterState state)
        {
            if (object.ReferenceEquals(target, null))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (object.ReferenceEquals(state, null))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var t = target.Value?.GetType().GetTypeInfo();
            return t?.IsClass ?? false;
        }

        public override T Transform(FieldData target, FormatterState state)
            => this.transform(target, state);
    }
}
