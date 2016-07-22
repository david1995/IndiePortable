// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagePart.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the PackagePart class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class PackagePart
    {

        public HeaderInformation PartHeader { get; private set; }


        public ReadOnlyCollection<RefByteObject> ByteObjects { get; private set; }
    }
}
