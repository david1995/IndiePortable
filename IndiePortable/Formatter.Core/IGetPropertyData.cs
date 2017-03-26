// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IGetPropertyData.cs" company="David Eiwen">
// Copyright © 2017 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IGetPropertyData interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Core
{

    public interface IGetPropertyData
    {
        object GetField(FieldName name);


        T GetFieldName<T>(FieldName name);


        bool HasField(FieldName name);
    }
}
