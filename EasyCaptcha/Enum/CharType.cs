using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCaptcha.Enum
{
    public enum CharType
    {
        /// <summary>
        /// Contains alphanumeric chars
        /// for reducing misunderstanding chars o and 0 eliminated from the output
        /// </summary>
        MIX,
        /// <summary>
        /// Contains only numbers
        /// for reducing misunderstanding char 0 eliminated from the output
        /// </summary>
        NUM
    }
}
