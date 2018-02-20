using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeCrypterU
{
    class UnknownFormatException: Exception
    {
        public UnknownFormatException()
        {

        }

        public UnknownFormatException(string message):base(message)
        {

        }
    }
}
