// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="NullRule.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the NullRule class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol2_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class NullRule<T>
        : ProtocolRule<T>
    {

        private readonly Func<T> transform;


        public NullRule(Func<T> transform)
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

            return object.ReferenceEquals(target.Value, null);
        }

        public override T Transform(FieldData target, FormatterState state)
            => this.transform();
    }
}
