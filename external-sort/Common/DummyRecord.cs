using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Common
{
    public class DummyRecord : IComparable<DummyRecord>, IParseable<DummyRecord>
    {
        private string _text;

        public DateTime DateTimeSent { get; set; }
        public Guid Data { get; set; }

        public DummyRecord()
        {
            
        }

        public DummyRecord(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text is null or empty", "text");
            }

            _text = text;
            Parse();
        }

        private void Parse()
        {
            DateTime dateTimeSent;
            Guid data;
            var split = _text.Split(new char[] { ',' } , StringSplitOptions.None);

            if (DateTime.TryParse(split[0], out dateTimeSent))
            {
                DateTimeSent = dateTimeSent;
            }

            if (Guid.TryParse(split[1], out data))
            {
                Data = data;
            }
        }

        public override string ToString()
        {
            return _text;
        }

        public int CompareTo(DummyRecord record)
        {
            if (this.DateTimeSent > record.DateTimeSent)
            {
                return -1;
            }

            if (this.DateTimeSent == record.DateTimeSent && this.Data == record.Data)
            {
                return 0;
            }

            return 1;
        }

        public void Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text is null or empty", "text");
            }

            _text = text;
            Parse();
        }
    }
}
