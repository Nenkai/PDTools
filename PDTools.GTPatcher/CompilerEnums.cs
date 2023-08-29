using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GTPatcher
{
    internal class CompilerEnums
    {
        public static Dictionary<uint, string> TokenEnum = new Dictionary<uint, string>()
        {
            { 0, "EOF" },

            { 33, "!" },
            { 37, "%" },
            { 40, "(" },
            { 41, ")" },
            { 42, "*" },
            { 43, "+" },
            { 45, "-" },
            { 46, "." },
            { 47, "/" },
            { 58, ":" },
            { 59, ";" },
            { 60, "<" },
            { 61, "=" },
            { 62, ">" },
            { 63, "?" },
            { 64, "@" },
            { 84, "raw string literal" },
            { 91, "[" },
            { 92, "\\" },
            { 93, "]" },
            { 94, "^" },
            { 124, "|" },
            { 126, "~" },
            { 258, "Boolean literal" },
            { 259, "Numeric Literal" },
            { 260, "Long literal" },
            { 261, "UInt literal" },
            { 262, "ULong literal" },
            { 263, "Float/Decimal literal" },
            { 265, "String Literal" },
            { 267, "Identifier" },
            { 268, "Binary Assign" },
            { 269, "Symbol literal" },
            { 270, "..." },
            { 271, "try" },
            { 272, "catch" },
            { 273, "throw" },
            { 274, "finally" },
            { 275, "var" },
            { 276, "undef" },
            { 277, "method" },
            { 278, "class" },
            { 279, "module" },
            { 280, "default" },
            { 281, "function" },
            { 282, "nil" },
            { 283, "if" },
            { 284, "else" },
            { 285, "do" },
            { 286, "while" },
            { 287, "switch" },
            { 288, "case" },
            { 289, "for" },
            { 290, "break" },
            { 291, "continue" },
            { 292, "return" },
            { 293, "yield" },
            { 294, "async" },
            { 295, "await" },
            { 296, "import" },
            { 297, "require" },
            { 298, "foreach" },
            { 299, "call" },
            { 300, "static" },
            { 301, "attribute" },
            { 302, "delegate" },
            { 303, "__FILE__" },
            { 304, "__LINE__" },
            { 305, "use_strict" },
            { 306, "no_strict" },
            { 307, "push_strict" },
            { 308, "pop_strict" },
            { 309, "dump" },
            { 310, "unknown" },
            { 311, "exec" },
            { 312, "link" },
            { 313, "current_module" },
            { 314, "include" },
            { 315, "??" },
            { 316, "||" },
            { 317, "&&" },
            { 318, "==" },
            { 319, "!=" },
            { 320, ">=" },
            { 321, "<=" },
            { 322, "<<" },
            { 323, ">>" },
            { 324, "**" },
            { 325, ".*" },
            { 326, "?.*" },
            { 331, "++" },
            { 332, "--" },
            { 333, "?." },
            { 334, "?[" },
            { 335, "::" },
        };
    }
}             
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              