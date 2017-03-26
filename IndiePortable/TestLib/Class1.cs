using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndiePortable.Communication.Messages;
using IndiePortable.Formatter;

namespace TestLib
{
    [IndiePortable.Formatter.Serializable]
    public class TextMessage
        : MessageBase
    {

        public TextMessage(string message)
        {
            this.Message = message;
        }

        protected TextMessage()
        {
        }

        protected TextMessage(ObjectDataCollection data)
            : base(data)
        {
            string message;
            if (!data.TryGetValue(nameof(this.Message), out message))
            {
                throw new ArgumentException();
            }

            this.Message = message;
        }

        public TextMessage(Guid identifier)
            : base(identifier)
        {
        }


        public string Message { get; }


        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.Message), this.Message);
        }
    }
}
