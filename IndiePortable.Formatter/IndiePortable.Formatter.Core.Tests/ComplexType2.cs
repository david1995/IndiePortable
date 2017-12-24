namespace IndiePortable.Formatter.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public class ComplexType2
    {
        public ComplexType2(string text, int value)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
            this.Value = value;
        }

        protected ComplexType2()
        {
        }

        public string Text { get; private set; }

        public int Value { get; private set; }
    }
}
