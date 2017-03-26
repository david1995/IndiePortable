// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ISetPropertyData.cs" company="David Eiwen">
// Copyright © 2017 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ISetPropertyData interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Core
{

    public interface ISetPropertyData
    {
        void AddField(FieldName name, object value);
    }
}
