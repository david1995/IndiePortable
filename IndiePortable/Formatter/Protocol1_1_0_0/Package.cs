// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="Package.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the Package class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class Package
    {

        public List<PackagePart> Parts { get; private set; }
    }
}
