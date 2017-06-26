// <copyright file="DateTimeHeaderField.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class DateTimeHeaderField
        : HttpHeaderField
    {
        public DateTimeHeaderField(string fieldName)
            : base(fieldName)
        {
        }

        public DateTime Value { get; set; }

        public override string GetRawValue()
            => this.Value.ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern);

        public override void SetRawValue(string value)
            => this.Value = DateTime.TryParseExact(
                                value,
                                DateTimeFormatInfo.InvariantInfo.RFC1123Pattern,
                                DateTimeFormatInfo.InvariantInfo,
                                DateTimeStyles.AdjustToUniversal,
                                out var res)
                          ? res
                          : throw new FormatException();
    }
}
