using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForthError
{
    // class for custom errors
    public class DoForthErrorException : Exception
    {
        public DoForthErrorException()
        {
        }

        public DoForthErrorException(string message)
            : base(message)
        {
            Console.WriteLine(message);
        }

        public DoForthErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
