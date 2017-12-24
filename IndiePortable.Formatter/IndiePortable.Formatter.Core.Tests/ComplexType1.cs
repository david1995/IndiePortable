namespace IndiePortable.Formatter.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ComplexType1
    {
        public ComplexType1(string text, int value)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
            this.Value = value;
        }

        public string Text { get; }

        public int Value { get; }
    }
}
