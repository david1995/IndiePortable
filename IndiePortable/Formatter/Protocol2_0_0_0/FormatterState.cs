// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatterState.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the FormatterState class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol2_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;





    public class FormatterState
    {
        
        private readonly IList<ObjectDataCollection> objectDataBacking;


        public FormatterState()
        {
            this.objectDataBacking = new List<ObjectDataCollection>();
        }


        public IList<ObjectDataCollection> ObjectData => this.objectDataBacking;
    }
}
