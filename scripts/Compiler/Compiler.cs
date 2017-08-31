using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


static class Compiler
{
    public const string EOP = "***EOP***";

    private static string _code;
    private static List<string> _icode;
    private static List<string> _tokens;
    private static char[] _operators = { '+', '-', '*', '/', '%', '^' };
    private static char[] _comOperators = { '>', '<', '=' };
    private static char[] _logicalOperators = { '|', '&', '!' };
    private static char[] _specialChars = { '{', '}', '(', ')', ';' };
    private static string[] _decKeywords = { "var", "int", "float" };
    private const char WS = ' ';
    private const char EOS = '\0';

    //error handling//
    private static string error;

    public static void setCode(string __code) { _code = __code + EOS; }

    public static bool compile()
    {
        tokenise();
        parserInit();
        if (!statements())
        {
            genErrorStatement();
            return false;
        }
        icodeGenInit();
        genCode();
        return true;
    }

    //**********tokenizer functions**************//
    public static bool tokenise()
    {
        _tokens = new List<string>();
        int _pointer = 0;
        while (_code[_pointer] != 0) /*0-end of string/null char*/
        {
            string __token = "";
            char __ch;

            //check for whitespaces and end of lines.
            if (_code[_pointer] == WS || isNewLine(_code[_pointer])) _pointer++;
            //remove comments
            else if (_code[_pointer] == '/' && (_code[_pointer + 1] != 0 && _code[_pointer + 1] == '/'))
            {
                while (_code[_pointer] != 0 && !isNewLine(_code[_pointer])) _pointer++;
            }
            //check for operators +,-,*,/,%//
            else if (isOperator(__ch = _code[_pointer]))
            {
                _tokens.Add(__ch.ToString());
                _pointer++;
            }

            //check for comparitions =,<,>
            else if (isComOperator(__ch = _code[_pointer]))
            {
                __token += __ch;
                _pointer++;
                if ((__ch = _code[_pointer]) == '=')
                {
                    __token += __ch;
                    _pointer++;
                }
                _tokens.Add(__token);
                __token = "";
            }
            //check for logical operator ! and comparition operator !=
            else if ((__ch = _code[_pointer]) == '!')
            {
                __token += __ch;
                _pointer++;
                if (_code[_pointer] == '=')
                {
                    __token += '=';
                    _pointer++;
                }
                _tokens.Add(__token);
                __token = "";
            }
            //check for logical operator |,&
            else if (isLogicalOperator(__ch = _code[_pointer]))
            {
                __token += __ch;
                _pointer++;
                if (isLogicalOperator(__ch = _code[_pointer]))
                {
                    __token += __ch;
                    _pointer++;
                }
                _tokens.Add(__token);
                __token = "";
            }
            //check for special chars ;,{,},(,)
            else if (isSpecialChar(__ch = _code[_pointer]))
            {
                _tokens.Add(__ch.ToString());
                _pointer++;
            }
            else if (_code[_pointer] == '"')
            {
                string str = "\"";
                _pointer++;
                while (_code[_pointer] != '"') { str += _code[_pointer++]; }
                str += _code[_pointer++];
                _tokens.Add(str);
            }
            //strings like keywords, variables, arguments
            else
            {
                bool __flag = false;
                while (isNewLine(_code[_pointer])) _pointer++;
                while (!isTokenEnd((__ch = _code[_pointer])))
                {
                    __token += __ch;
                    _pointer++;
                    //Deug//
                    //if (_pointer+1 >= _code.Length) return true;
                    __flag = true;
                }
                if (__flag) _tokens.Add(__token);
            }
        }
        return true;
    }

    private static bool isTokenEnd(char ch)
    {
        if (isOperator(ch) || isComOperator(ch) || isLogicalOperator(ch) || isSpecialChar(ch) || ch == ' ' || ch == EOS || isNewLine(ch)) return true;
        return false;
    }
    private static bool isOperator(char ch)
    {
        for (int i = 0; i < _operators.Length; i++)
            if (ch == _operators[i]) return true;
        return false;
    }
    private static bool isComOperator(char ch)
    {
        for (int i = 0; i < _comOperators.Length; i++)
            if (ch == _comOperators[i]) return true;
        return false;
    }
    private static bool isLogicalOperator(char ch)
    {
        for (int i = 0; i < _logicalOperators.Length; i++)
            if (ch == _logicalOperators[i]) return true;
        return false;
    }
    private static bool isSpecialChar(char ch)
    {
        for (int i = 0; i < _specialChars.Length; i++)
            if (ch == _specialChars[i]) return true;
        return false;
    }
    private static bool isNewLine(char ch)
    {
        //Console.Write(ch + "\t");
        if (ch == '\n' || ch == '\r')
        {
            return true;
        }
        return false;
    }
    private static bool isEndOfString(char ch)
    {
        if (ch == EOS) return true;
        return false;
    }

    public static void printTokens()
    {
        for (int i = 0; i < _tokens.Count; i++)
            Console.WriteLine(_tokens[i]);
    }

    //*********parser functions**********//
    private static int _pointer = 0;
    private static List<KeyValuePair<string, Type>> _variables;
    public static bool parser()
    {
        parserInit();
        if (!statements())
        {
            genErrorStatement();
            return false;
        }
        return true;
    }
    private static void parserInit()
    {
        error += "";
        _pointer = 0;
        _variables = new List<KeyValuePair<string, Type>>();
    }
    private static bool statements()
    {
        while (_pointer < _tokens.Count)
        {
            string __token = _tokens[_pointer];
            char __ch = __token[0];
            if (isComOperator(__ch) || isOperator(__ch)) { error += "invalid token '" + __token + "'"; return false; }
            if (__token != "{" && __token != "}")
            {
                if (!statement())
                    return false;
            }
            else if (__token == "}") return true;
            else { if (!block()) return false; }
        }
        return true;
    }
    private static bool statement()
    {
        if (_tokens[_pointer] == ";") { _pointer++; return true; }
        else if (_tokens[_pointer] == "if") { if (!ifParse()) { return false; } }
        else if (_tokens[_pointer] == "for") { if (!forParse()) { return false; } }
        else if (_tokens[_pointer] == "while") { if (!whileParse()) { return false; } }
        else if (isDeclaratorKeyword(_tokens[_pointer])) { if (!declarator()) { return false; } }
        else if (isFunction(_tokens[_pointer])) { if (!function()) { return false; } }
        else if (isVariable(_tokens[_pointer])) { if (!expression(false)) { return false; } }
        else { error += "invalid token:" + _tokens[_pointer]; return false; }
        return true;
    }
    private static bool block()
    {
        _pointer++;
        //Console.WriteLine("block");
        if (_tokens[_pointer] == "}") return true;
        if (!statements()) return false;
        if (_tokens[_pointer] != "}") { error = "missing '}'.\n"; return false; }
        _pointer++;
        return true;
    }
    private static bool forParse()
    {
        _pointer++;
        Console.WriteLine("for parse");
        if (_tokens[_pointer++] != "(") { error = "'(' is missing for 'for'.\n"; return false; }
        if (declarator())
        {
            if (_tokens[_pointer++] != ";") { error = "invalid declaration syntax for 'for'.\n"; return false; }
        }
        else { if (_tokens[_pointer++] != ";") { error = "missing ';' inside 'for' after  declaration.\n"; return false; } }

        if (comExpression(false))
        {
            if (_tokens[_pointer++] != ";") { error = "missing ';' inside 'for' after comparition.\n"; return false; }
        }
        else { if (_tokens[_pointer++] != ";") { error += "invalid compariotion syntax for 'for'.\n"; return false; } }
        if (!expression(true)) { error += "invalid expression syntax for 'for'.\n"; return false; }
        if (_tokens[_pointer++] != ")") { error = "')' is missing for 'for'.\n"; return false; }
        if (!block())
        {
            return false;
        }
        return true;
    }
    private static bool whileParse()
    {
        _pointer++;
        if (_tokens[_pointer++] != "(") { error = "'(' is missing for 'while'.\n"; return false; }
        if (!comExpression(true)) { error += "::while parse comparision error.\n"; return false; }
        if (_tokens[_pointer++] != ")") { error = "')' missing for 'while'.\n"; return false; }
        if (!block()) { error = "missing '}' at end of 'while' loop.\n"; return false; }
        return true;
    }
    private static bool ifParse()
    {
        _pointer++;
        if (_tokens[_pointer++] != "(") { error = "'(' is missing for 'if'."; return false; }
        if (!comExpression(true)) { error += "::if parse error."; return false; }
        if (_tokens[_pointer++] != ")") { error = "')' missing for 'if'."; return false; }
        if (!block()) { error = "missing '}' at end of 'if' block.\n"; return false; }
        if (isPointerInRange() && _tokens[_pointer] == "else")
        { if (!elseParse()) { return false; } }
        return true;
    }
    private static bool elseParse()
    {
        if (_tokens[_pointer] == "else")
        {
            _pointer++;
            if (_tokens[_pointer] == "{")
            {
                if (!block()) { error = "missing '}' at end of 'else' block.\n"; return false; }
            }
            else if (_tokens[_pointer] == "if")
            {
                if (!ifParse()) { return false; }
            }
        }
        return true;
    }
    //updates the tokens//
    //all uninary - are replaced with #.
    private static bool expression(bool forFlag)
    {
        _pointer++;
        if (_tokens[_pointer] != "=") { error = "expression missing '='\n"; return false; }
        _pointer++;
        int state = 0;
        int depth = 0;
        while (true)
        {
            switch (state)
            {
                //case for '('
                case 0:
                    if (_tokens[_pointer] == "(") { state = 0; depth++; _pointer++; }
                    else if (_tokens[_pointer] == "-") { state = 2; _tokens[_pointer] = "#"; _pointer++; }
                    else if (varNumFunc()) { state = 1; }
                    else { error = "expression must contain '(' or '-' or a variable or a constant or a function after '('\n"; return false; }
                    break;
                //case for variable and ')'
                case 1:
                    if (_tokens[_pointer] == ";" || (forFlag && _tokens[_pointer] == ")" && _tokens[_pointer + 1] == "{")) { state = 3; }
                    else if (_tokens[_pointer] == ")") { state = 1; depth--; _pointer++; }
                    else if (operators()) { state = 2; }
                    else { error = "expression must contain ')' or ';' or an operator after (" + _tokens[_pointer - 1] + ") ')' or variable or constant or a function.\n"; return false; }
                    break;
                //case for '-' and operator.
                case 2:
                    if (_tokens[_pointer] == "(") { state = 0; depth++; _pointer++; }
                    else if (varNumFunc()) { state = 1; }
                    else { error = "expression must contain '(' or '-' or a variable or a constant or a function after (" + _tokens[_pointer - 1] + ") an operator\n"; return false; }
                    break;
                //case for ';'.does depth check.
                case 3:
                    if (depth != 0)
                    {
                        if (depth > 0) { error = "missing ')' in the expression.\n"; }
                        else { error = "too many ')' in the expression\n"; }
                        return false;
                    }
                    else return true;

            }
        }
        //if (!varNumFunc()) { return false; }
        //if (_tokens[_pointer] == ";" || _tokens[_pointer] == ")") return true;
        //else {
        //    if (operators())
        //    {
        //        if (!varNumFunc()) { return false; }
        //        //if (_tokens[_pointer] != ";") { error += "invalid expression.";return false; }
        //    }
        //    else { error += "invalid expression"; return false; }
        //}
    }
    //if and while have '){' as 'End Of Expression'.
    private static bool comExpression(bool ifWhileFlag)
    {
        //if (!varNum()) { error += "comparition must contain a variable or number\n"; return false; }
        //if (!comOperators()) { error += "comparition must contain an operator\n"; return false; }
        //if (!varNum()) { error += "comparition must contain a variable or number\n"; return false; }
        //return true;
        int state = 0;
        int depth = 0;
        while (true)
        {
            if (_tokens[_pointer] == "-") { error = "unary - is not allowed in comparitions!!!"; return false; }
            switch (state)
            {
                //case for '('
                case 0:
                    if (_tokens[_pointer] == "(") { state = 0; depth++; _pointer++; }
                    else if (_tokens[_pointer] == "!") { state = 5; _pointer++; }
                    else if (varNumFunc()) { state = 3; }
                    else { error = "expression must contain '(' or '!' or a variable or a constant or a function after '(' or at beggining of expression\n"; return false; }
                    break;
                //case for comparition operator.
                case 1:
                    if (varNumFunc()) { state = 3; }
                    else { error = "expression must contain a variable or a constant or a function after <,>,<=,>=,=,==,!= operators\n"; return false; }
                    break;
                //case for ')'
                case 2:
                    if (_tokens[_pointer] == ";" || (ifWhileFlag && _tokens[_pointer] == ")" && _tokens[_pointer + 1] == "{")) { state = 6; }
                    else if (logicalOperators()) { state = 4; }
                    else { error = "expression must contain '){' or ';' or a logical operator(||,&&) after )" + _tokens[_pointer - 1] + ") ')' or variable or constant or a function.\n"; return false; }
                    break;
                //case for variable
                case 3:
                    if (_tokens[_pointer] == ";" || (ifWhileFlag && _tokens[_pointer] == ")" && _tokens[_pointer + 1] == "{")) { state = 6; }
                    else if (_tokens[_pointer] == ")") { state = 2; depth--; _pointer++; }
                    else if (comOperators()) { state = 1; }
                    else if (logicalOperators()) { state = 4; }
                    else { error = "expression must contain ')' or ';' or '){' or a comparition operator(<,>,<=,>=,=,==,!=) or logical operator(||,&&) after variable or constant or function " + _tokens[_pointer - 1] + ") ')' or variable or constant or a function.\n"; return false; }
                    break;
                //case for logical operator.
                case 4:
                    if (_tokens[_pointer] == "(") { state = 0; depth++; _pointer++; }
                    else if (varNumFunc()) { state = 3; }
                    else if (_tokens[_pointer] == "!") { state = 5; _pointer++; }
                    else { error = "expression must contain '(' or a variable or a constant or a function not operator(!) after logical operator(||,&&) " + _tokens[_pointer - 1] + ") an operator\n"; return false; }
                    break;
                //case for '!'.
                case 5:
                    if (_tokens[_pointer] == "(") { state = 0; depth++; _pointer++; }
                    else { error = "expression must contain '(' after not operator(!) \n"; return false; }
                    break;
                //case for ';'.does depth check.
                case 6:
                    if (depth != 0)
                    {
                        if (depth > 0) { error = "missing ')' in the expression.\n"; }
                        else { error = "too many ')' in the expression\n"; }
                        return false;
                    }
                    else return true;

            }
        }
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!declarator under construction//
    private static bool declarator()
    {
        if (!isDeclaratorKeyword(_tokens[_pointer])) { error = "undefined declaration keyword" + _tokens[_pointer] + ".\n"; return false; }
        string __dataType = _tokens[_pointer];
        _pointer++;
        if (!isVariable(_tokens[_pointer]))
        {
            string __var = _tokens[_pointer];
            _pointer++;
            Type __type;
            if (__dataType == "int")
            {
                __type = Type.INT;
                if (_tokens[_pointer] == "(")
                {
                    _pointer++;
                    if (_tokens[_pointer] == ")") { error = "array declaration error:too few arguments.\n"; return false; }
                    string[] local = _tokens[_pointer].Split(',');
                    if (local.Length == 1) __type = Type.INT_ARRAY_1D;
                    else if (local.Length == 2) __type = Type.INT_ARRAY_2D;
                    else { error = "array declaration error:too many arguments.\n"; return false; }
                    _pointer++;
                    if (_tokens[_pointer] != ")") { error = "array declaration error:missing ')'.\n"; return false; }
                }
            }
            else if (__dataType == "float")
            {
                __type = Type.FLOAT;
                if (_tokens[_pointer] == "(")
                {
                    _pointer++;
                    if (_tokens[_pointer] == ")") { error = "array declaration error:too few arguments.\n"; return false; }
                    string[] local = _tokens[_pointer].Split(',');
                    if (local.Length == 1) __type = Type.FLOAT_ARRAY_1D;
                    else if (local.Length == 2) __type = Type.FLOAT_ARRAY_2D;
                    else { error = "array declaration error:too many arguments.\n"; return false; }
                    _pointer++;
                    if (_tokens[_pointer] != ")") { error = "array declaration error:missing ')'.\n"; return false; }
                }
            }
            else __type = Type.INT;
            createVariable(__var, __type);
            //return false;
        }
        else _pointer++;
        if (_tokens[_pointer] == ";") return true;
        else if (_tokens[_pointer++] == "=")
        {
            if (_variables[_variables.Count - 1].Value == Type.INT) { if (!integer()) { error = "initialization error: expecting an integer value.\n"; return false; } }
            else if (_variables[_variables.Count - 1].Value == Type.FLOAT) { if (!floatingPoint()) { error = "initialization error: expecting an float value.\n"; return false; } }
            else { error = "array initialization error: cannot initialize an array at declaration.\n"; return false; }

            if(_tokens[_pointer]!= ";") { error = "missing ';' after declaration.";return false; }
        }
        //if(_tokens[_pointer]=="=")
        return true;
    }
    private static bool function()
    {
        if (!isFunction(_tokens[_pointer])) { return false; }
        _pointer++;
        if (!arguments()) { return false; }
        return true;
    }
    //does some semantic check.
    private static bool arguments()
    {
        if (Library.getArgs() == "") return true;
        string[] __args = Library.getArgs().Split(':');
        int i = 0;
        while (_tokens[_pointer] != ";")
        {
            if (isUninaryOp(_tokens[_pointer]))
            {
                string op = _tokens[_pointer];
                _tokens.RemoveAt(_pointer);
                _tokens[_pointer] = op + _tokens[_pointer];
            }
            if (i >= __args.Length) { error = "too many arguments:" + i + "::expecting arguments:" + __args.Length + "\n"; return false; }
            if (!argument(__args[i])) { return false; }
            i++;
        }
        if (i < __args.Length) { error = "too few arguments:" + i + "::expecting arguments:" + __args.Length + "\n"; return false; }
        return true;
    }
    //does semantic check
    private static bool argument(string __arg)
    {
        if (__arg == "int" || __arg == "float")
        {
            if (!varNum()) { return false; }
        }
        else if (__arg == "direction")
        {
            if (!directions()) { return false; }
        }
        else if (__arg == "var")
        {
            if (!isVariable(_tokens[_pointer])) { return false; }
            _pointer++;
        }
        else if (__arg == "string")
        {
            if (!variable()) { return false; }
        }
        else if (__arg == "string2")
        {
            if (!isString()) { return false; }
            _pointer++;
        }
        else if (__arg == "array2df")
        {
            if (!isArray2dF()) { error = "exprecting a 2 dimentional floating array.\n"; return false; }
            _pointer++;
        }
        return true;
    }
    //points to next token in case of success.//
    private static bool varNum()
    {

        if (!isVariable(_tokens[_pointer]))
        {
            if (!floatingPoint()) { return false; }
        }
        else { _pointer++; }
        return true;
    }
    private static bool varNumFunc()
    {
        if (!isMathFunction(_tokens[_pointer]))
        {
            if (!varNum()) { return false; }
        }
        return true;
    }
    private static bool variable()
    {
        string __token = _tokens[_pointer];
        for (int i = 0; i < __token.Length; i++)
        {
            char __ch = __token[i];
            if (!((__ch >= 'a' && __ch <= 'z') || (__ch >= 'A' && __ch <= 'Z') || (__ch >= '0' && __ch <= '9') || __ch == '_'))
            {
                error = "invalid variable name '" + __token + "'. variable name consists of upper and lower case alphabets and numbers only. but found '" + __ch + "'.\n";
                return false;
            }
        }
        _pointer++;
        return true;
    }
    private static bool operators()
    {
        for (int i = 0; i < _operators.Length; i++)
        {
            if (_tokens[_pointer] == _operators[i].ToString())
            {
                _pointer++;
                return true;
            }
        }
        error = "invalid operator '" + _tokens[_pointer] + "'. accepts +,-,*,/,% and ^ only.\n";
        return false;
    }
    private static bool comOperators()
    {
        string __token = _tokens[_pointer];
        if (__token != ">" &&
           __token != "<" &&
           __token != "=" &&
           __token != "<=" &&
           __token != ">=" &&
           __token != "==" &&
           __token != "!=")
        {
            error = "invalid operator '" + _tokens[_pointer] + "'. accepts >,<,=,==,!=,<= and >= only.\n";
            return false;
        }
        _pointer++;
        return true;
    }
    private static bool logicalOperators()
    {
        string __token = _tokens[_pointer];
        if (__token != "||" &&
           __token != "&&")
        {
            error = "invalid operator '" + _tokens[_pointer] + "'. accepts >,<,=,==,!=,<= and >= only.\n";
            return false;
        }
        _pointer++;
        return true;
    }
    private static bool directions()
    {
        string __token = _tokens[_pointer];
        if (__token.ToLower() != "up" &&
            __token.ToLower() != "down" &&
            __token.ToLower() != "left" &&
            __token.ToLower() != "right" &&
            __token.ToLower() != "forward" &&
            __token.ToLower() != "backward")
        { error = "invalid direction " + __token + "'. accepts up,down,left,right,forward and backwards only.\n"; ; return false; }
        _pointer++;
        return true;
    }
    //points to the next token incase of success.//
    private static bool integer()
    {
        string __token = _tokens[_pointer];
        if (isInteger(__token))
        {
            _pointer++;
            return true;
        }
        else { return false; }
    }
    //points to the next token incase of success.//
    private static bool floatingPoint()
    {
        string __token = _tokens[_pointer];
        if (isFloatingPoint(__token))
        {
            _pointer++;
            return true;
        }
        else { return false; }
    }

    //support functions//
    //!!pointer incrementors//
    //checks from "[" and points after "]".//
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!currently not in use//
    private static bool isArray()
    {
        if (_tokens[_pointer] == "[")
        {
            _pointer++;
            if (integer())
            {
                if (_tokens[_pointer] == "]")
                {
                    _pointer++;
                    return true;
                }
            }

        }
        return false;
    }
    //checks wether given variable exists//
    //points to the last token of the variable. usually modifies the pointer in case of array.//
    private static bool isVariable(string __token)
    {
        for (int i = 0; i < _variables.Count; i++)
        {
            //check for variable existance//
            if (__token == _variables[i].Key)
            {
                //check for arrray variable//
                //incase of array checks for number of arguments and type of arguments to be integer.//
                if (_tokens[_pointer + 1] == "(")
                {
                    //validating if given variable is declared array//
                    if (_variables[i].Value == Type.INT || _variables[i].Value == Type.FLOAT) { error = __token + ":is not an array."; return false; }
                    //incase of array make sure it has atleast one argument//
                    if (_tokens[_pointer + 2] == ")") { error = "array usage error:no arguments."; return false; }
                    //collecting the arguments of array delimited by comma.//
                    string[] local = _tokens[_pointer + 2].Split(',');
                    //check for 1D array//
                    if (_variables[i].Value == Type.FLOAT_ARRAY_1D || _variables[i].Value == Type.INT_ARRAY_1D)
                    {
                        if (local.Length != 1) { error = __token + " accepts exactly one argument."; return false; }
                        //checkk for integer datatype for argument.//
                        if (!isInteger(local[0]))
                        {
                            if (!isIntegerVariable(local[0])) { error = __token + " accepts only integer data type as argument."; return false; }
                        }
                        //check for array syntax: the closing brace.//
                        if (_tokens[_pointer + 3] != ")") { error = "array usage error:missing ')'."; return false; }
                        //updating pointer to last token of array.//
                        _pointer += 3;
                    }
                    //check for 2D array//
                    if (_variables[i].Value == Type.FLOAT_ARRAY_2D || _variables[i].Value == Type.INT_ARRAY_2D)
                    {
                        if (local.Length != 2) { error = __token + " accepts exactly two argument."; return false; }
                        //checkk for integer datatype for argument.//
                        if (!isInteger(local[0]))
                        {
                            if (!isIntegerVariable(local[0])) { error = __token + " accepts only integer data type as argument."; return false; }
                        }
                        if (!isInteger(local[1]))
                        {
                            if (!isIntegerVariable(local[1])) { error = __token + " accepts only integer data type as argument."; return false; }

                        }                        //check for array syntax: the closing brace.//
                        if (_tokens[_pointer + 3] != ")") { error = "array usage error:missing ')'."; return false; }
                        //updating pointer to last token of array.//
                        _pointer += 3;
                    }
                }
                return true;
            }
        }
        //error += "variable '"+__token+"' not declared.";
        return false;
    }
    //checks for math function. General syntax ex:<func name>(<varnum>)//
    //points to next token incase of success.//
    private static bool isMathFunction(string __token)
    {
        if (Library.isMathFunc(__token))
        {
            _pointer++;
            if (_tokens[_pointer] != "(") { error += "invalid function definition:" + __token + "."; return false; }
            _pointer++;
            if (!varNum()) { return false; }
            //_pointer++;
            if (_tokens[_pointer] != ")") { error += "invalid function definition:" + __token + "."; return false; }
            _pointer++;
            return true;
        }
        return false;
    }

    //doesnt increment pointer//
    private static bool isPointerInRange()
    {
        if (_pointer < _tokens.Count) return true;
        return false;
    }
    private static bool isFunction(string __token)
    {
        return Library.isFunc(__token);
        //return true;
    }
    private static bool isInteger(string __token)
    {
        for (int i = 0; i < __token.Length; i++)
        {
            char __ch = __token[i];
            if (i == 0 && isUninaryOp(__ch.ToString())) continue;
            if (!((__ch >= '0' && __ch <= '9')))
            {
                //error = "invalid number '" + __token + "'\n";
                return false;
            }
        }
        return true;
    }
    private static bool isFloatingPoint(string __token)
    {
        bool dotFlag = true;

        for (int i = 0; i < __token.Length; i++)
        {
            char __ch = __token[i];
            if (i == 0 && isUninaryOp(__ch.ToString())) continue;
            if (dotFlag && __ch == '.') { dotFlag = false; continue; }
            if (!((__ch >= '0' && __ch <= '9')))
            {
                //error = "invalid floating number '" + __token + "'\n";
                return false;
            }
        }

        return true;
    }
    private static bool isIntegerVariable(string __var)
    {
        for (int i = 0; i < _variables.Count; i++)
        {
            if (__var == _variables[i].Key && _variables[i].Value == Type.INT) return true;
        }
        return false;
    }
    private static bool isFloatVariable(string __var)
    {
        for (int i = 0; i < _variables.Count; i++)
        {
            if (__var == _variables[i].Key && _variables[i].Value == Type.FLOAT) return true;
        }
        return false;
    }
    private static bool isUninaryOp(string __token)
    {
        if (__token == "-" || __token == "+") return true;
        return false;
    }
    private static bool isDeclaratorKeyword(string __token)
    {
        for (int i = 0; i < _decKeywords.Length; i++)
            if (__token == _decKeywords[i]) return true; ;
        return false;
    }
    private static bool isArray2dF()
    {
        for (int i = 0; i < _variables.Count; i++)
        {
            if (_tokens[_pointer] == _variables[i].Key && _variables[i].Value == Type.FLOAT_ARRAY_2D) return true;
        }
        return false;
    }
    private static bool isString()
    {
        string str = _tokens[_pointer];
        if (str[0] == '"' && str[str.Length - 1] == '"') return true;
        return false;
    }
    private static void createVariable(string __token, Type __type)
    {
        KeyValuePair<string, Type> tmp = new KeyValuePair<string, Type>(__token, __type);
        _variables.Add(tmp);
        return;
    }
    //********error handling*********//
    public static string getError() { return error; }
    private static void genErrorStatement()
    {
        int i;
        for (i = _pointer - 1; i >= 0 && _tokens[i] != ";" && _tokens[i] != "{"; i--) ;
        string str = "";
        i++;
        for (; i < _tokens.Count && _tokens[i] != ";" && _tokens[i] != "}" && _tokens[i] != "{"; i++) str += _tokens[i] + " ";
        error = "error at:" + str + "\nerror:\n" + error;
    }

    //*************intermediate code generation*****************//
    public static void iCodeGenerator()
    {
        icodeGenInit();
        genCode();
    }
    private static void icodeGenInit()
    {
        _pointer = 0;
        _icode = new List<string>();
        //creating default variable '$tmp0 = -1"//
        //used for handling uninary operator '-'//
        _icode.Add("var int $tmp0");
        _icode.Add("arithmatic $tmp0 0 1 -");
    }
    private static void genCode()
    {
        generate();
        _icode.Add(EOP);
    }
    private static void generate()
    {
        int size = _tokens.Count;
        while (_pointer < size)
        {

            if (_tokens[_pointer] == ";") { _pointer++; }
            else if (_tokens[_pointer] == "if") { pushIf(); }
            else if (_tokens[_pointer] == "for") { pushFor(); }
            else if (_tokens[_pointer] == "while") { pushWhile(); }
            else if (isDeclaratorKeyword(_tokens[_pointer])) { pushVar(); }
            else if (isFunction(_tokens[_pointer])) { pushFunction(); }
            else if (_tokens[_pointer] == "}")
            {
                return;
            }
            else { pushExpression(); }
        }
    }

    private static void pushVar()
    {
        string str = "var " + _tokens[_pointer++] + " ";
        string var = getVariable();
        str += var;
        _icode.Add(str);
        if (_tokens[_pointer] == ";") { return; }
        str = "arithmatic ";
        str += var + " ";
        str += _tokens[++_pointer] + " 0 +";
        _pointer++;
        _icode.Add(str);
    }
    private static void pushIf()
    {
        bool ifFlag = false;

        if (_tokens[_pointer - 1] != "else") { ifFlag = true; }
        //points to first token after if(.//
        _pointer += 2;
        string str = "if ";
        str += pushComExpression();
        _icode.Add(str);
        str = "branch -1";
        _icode.Add(str);
        int addr = _icode.Count - 1;

        _pointer += 2;
        generate();
        int branch = _icode.Count;
        _icode[addr] = "branch " + branch;

        _pointer++;
        if (isPointerInRange() && _tokens[_pointer] == "else")
        {
            _icode.Add("branch -1");
            branch = _icode.Count;
            _icode[addr] = "branch " + branch;
            addr = _icode.Count - 1;
            _pointer++;

            if (_tokens[_pointer] == "{") _pointer++;

            generate();
            branch = _icode.Count;
            _icode[addr] = "branch " + branch;

        }
        else _pointer--;

        if (ifFlag)
            _pointer++;
    }
    private static void pushFor()
    {
        //current pointer to 'for' change to 'var' in 'for ( var'
        _pointer += 2;
        pushVar();

        //push the if statement. 
        ///current:";"
        ///statement:";<var> <op> <var>"
        _pointer++;
        int loopStartPtr = _icode.Count;

        string str = "if ";
        str += pushComExpression();
        _icode.Add(str);

        //add branch to jump to end of loop
        str = "branch -1";
        _icode.Add(str);
        int addr = _icode.Count - 1;

        //record the pointer to increment/decrement expression to push at the end of for loop.
        int tmpPtr = _pointer + 1;
        //progress pointer to start of loop.
        //points to first token after "{".
        while (_tokens[_pointer++] != "{") ;
        //recersively execute all statements in loop.
        generate();

        //push the increment/decrement expression.
        //record current pointer and change the pointer to tmp expression pointer.
        //call push expression and restore the pointer.
        int tmpPtr2 = _pointer;
        _pointer = tmpPtr;
        pushForExpression();
        _pointer = tmpPtr2;

        //add branch statement to point to loop if condition.
        _icode.Add("branch " + (loopStartPtr));

        //update branch statement at start to point the end of loop.
        int branch = _icode.Count;
        _icode[addr] = "branch " + branch;

        //Point to first token after loop end("}").
        _pointer += 1;

    }
    private static void pushWhile()
    {
        //push the if statement. 
        ///current:"while"
        ///statement:"while ( <var> <op> <var>"
        int loopStartPtr = _icode.Count;
        _pointer += 2;
        string str = "if ";
        str += pushComExpression();
        _icode.Add(str);

        str = "branch -1";
        _icode.Add(str);
        int addr = _icode.Count - 1;

        _pointer += 2;
        generate();
        _icode.Add("branch " + (loopStartPtr));
        int branch = _icode.Count;
        _icode[addr] = "branch " + branch;
        _pointer++;
    }
    private static void pushFunction()
    {
        string str = "function ";
        push(str);
    }
    private static void pushForExpression()
    {
        string str = "arithmatic ";
        str += getVariable() + " ";
        _pointer++;

        str += getVariable() + " ";
        if (_tokens[_pointer] == ")")
        {
            str += "0 +";
            //_pointer += 3;
        }
        else
        {
            string op = _tokens[_pointer++];
            str += getVariable() + " ";
            str += op;
            //_pointer += 3;
        }
        _icode.Add(str);
    }
    private static void pushExpression()
    {
        string str = "arithmatic ";
        str += getVariable() + " ";
        _pointer++;
        List<string> infix = getInfix();
        List<string> postfix = getPostfix(infix);
        str += pushSubExp(postfix) + " 0 +";

        _icode.Add(str);
        //push();
    }
    //converts uninary '-' from '#' to '*'.since we multiply number with -1.
    private static string pushSubExp(List<string> postfix)
    {
        string var = "";
        if (postfix.Count == 1) var = postfix[0];
        else
        {
            int index = 1;
            Stack<string> stack = new Stack<string>();
            for (int i = 0; i < postfix.Count; i++)
            {
                string token = postfix[i];
                if (token == "#") { token = "*"; }
                if (isOperator(token))
                {
                    string rop = stack.Pop();
                    string lop = stack.Pop();
                    string lhs = "$tmp" + index;
                    index++;

                    //if (!isVariable(lhs))
                    //{
                    //    createVariable(lhs, Type.FLOAT);
                    //    _icode.Add("var float " + lhs);
                    //}

                    string subexp = "arithmatic " + lhs + " " + lop + " " + rop + " " + token;
                    _icode.Add(subexp);
                    stack.Push(lhs);
                }
                else stack.Push(token);
            }
            var = stack.Pop(); ;
        }
        return var;
    }
    private static string pushComExpression()
    {

        List<string> infix = getInfix();
        List<string> postfix = getPostfixComparision(infix);
        return pushSubComExp(postfix);
    }
    private static string pushSubComExp(List<string> postfix)
    {
        string var = "";
        if (postfix.Count == 1) var = postfix[0];
        else
        {
            int index = 1;
            Stack<string> stack = new Stack<string>();
            for (int i = 0; i < postfix.Count; i++)
            {
                string token = postfix[i];
                if (token == "!")
                {
                    string op = stack.Peek();
                    string subexp = "comparision " + op + " " + op + " " + op + " " + token;
                    _icode.Add(subexp);
                }
                else if (isComparisionOperator(token) || isLogicalOperator(token))
                {
                    string rop = stack.Pop();
                    string lop = stack.Pop();
                    string lhs = "$b" + index;
                    index++;

                    //if (!isVariable(lhs))
                    //{
                    //    createVariable(lhs, Type.FLOAT);
                    //    _icode.Add("var float " + lhs);
                    //}

                    string subexp = "comparision " + lhs + " " + lop + " " + rop + " " + token;
                    _icode.Add(subexp);
                    stack.Push(lhs);
                }
                else stack.Push(token);
            }
            var = stack.Pop(); ;
        }
        return var;
    }
    //support functions//
    //modifies pointer//
    //input:pointer to variable.//
    //End: pointer to next token after variable//
    private static string getVariable()
    {
        string var = _tokens[_pointer++];
        if (_tokens[_pointer] == "(")
        {
            var += _tokens[_pointer++];//push '('
            var += _tokens[_pointer++];//push "array indeices"
            var += _tokens[_pointer++];//push ")"
        }
        //Console.WriteLine("debug:" + var);
        return var;
    }
    private static void push(String str)
    {
        bool varFlag = false;
        for (int i = 0; _tokens[_pointer] != ";"; i++, _pointer++)
        {
            varFlag = false;
            for (int j = 0; j < _variables.Count; j++)
            {
                if (_tokens[_pointer] == _variables[j].Key)
                {
                    varFlag = true;
                    str += _tokens[_pointer];
                    if (_tokens[_pointer + 1] == "(")
                    {
                        _pointer++;
                        str += _tokens[_pointer++];//(
                        str += _tokens[_pointer++];//index
                        str += _tokens[_pointer];//)
                    }

                }
            }
            if (!varFlag) str += _tokens[_pointer];
            if (_tokens[_pointer + 1] != ";") str += " ";
        }
        //str += ";";
        //if (str[str.Length - 1] == ' ') str.Remove(str.Length - 1);
        _icode.Add(str);
    }
    private static List<string> getInfix()
    {
        List<string> infix = new List<string>();
        while (true)
        {
            if (_tokens[_pointer] == ";" || (_tokens[_pointer] == ")" && _tokens[_pointer + 1] == "{")) break;
            else if (_tokens[_pointer] == "(" || _tokens[_pointer] == ")")
            {
                infix.Add(_tokens[_pointer]);
                _pointer++;
            }
            else if (_tokens[_pointer] == "#") { infix.Add("$tmp0"); infix.Add("#"); _pointer++; }
            else if (operators() || comOperators() || logicalOperators()) infix.Add(_tokens[_pointer - 1]);
            else if (_tokens[_pointer] == "!") { infix.Add("!"); _pointer++; }
            else
            {
                infix.Add(getVariable());
            }
        }
        return infix;
    }
    private static List<string> getPostfix(List<string> infix)
    {
        List<string> postfix = new List<string>();
        Stack<string> stack = new Stack<string>();

        for (int i = 0; i < infix.Count; i++)
        {
            string token = infix[i];
            if (token == "(") stack.Push(token);
            else if (isPrecedLevel4(token)) stack.Push(token);
            else if (isPrecedLevel3(token))
            {
                while (stack.Count > 0 && isPrecedLevel4(stack.Peek())) postfix.Add(stack.Pop());
                stack.Push(token);
            }
            else if (isPrecedLevel2(token))
            {
                while (stack.Count > 0 && (isPrecedLevel3(stack.Peek()) || isPrecedLevel4(stack.Peek()))) postfix.Add(stack.Pop());
                stack.Push(token);
            }
            else if (isPrecedLevel1(token))
            {
                while (stack.Count > 0 && (isPrecedLevel2(stack.Peek()) || stack.Peek() == "^")) postfix.Add(stack.Pop());
                stack.Push(token);
            }
            else if (token == ")")
            {
                while (stack.Count > 0 && stack.Peek() != "(") postfix.Add(stack.Pop());
                stack.Pop();
            }
            else postfix.Add(token);
        }
        while (stack.Count > 0) { postfix.Add(stack.Pop()); }
        return postfix;
    }

    private static List<string> getPostfixComparision(List<string> infix)
    {
        List<string> postfix = new List<string>();
        Stack<string> stack = new Stack<string>();

        for (int i = 0; i < infix.Count; i++)
        {
            string token = infix[i];
            if (token == "(" || token == "!" || isComparisionOperator(token)) stack.Push(token);
            else if (isLogicalOperator(token))
            {
                while (stack.Count > 0 && (isComparisionOperator(stack.Peek()) || isLogicalOperator(stack.Peek()) || stack.Peek() == "!"))
                    postfix.Add(stack.Pop());
                stack.Push(token);
            }
            else if (token == ")")
            {
                Stack<string> tmp = new Stack<string>();
                while (stack.Count > 0 && stack.Peek() != "(")
                    postfix.Add(stack.Pop());
                stack.Pop();
            }
            else postfix.Add(token);
        }
        while (stack.Count > 0) { postfix.Add(stack.Pop()); }
        return postfix;
    }

    //support function without pointer modifications.//
    //support for postfix exp conversion.
    private static bool isPrecedLevel4(string op)
    {
        if (op == "#") return true;
        return false;
    }
    private static bool isPrecedLevel3(string op)
    {
        if (op == "^") return true;
        return false;
    }
    private static bool isPrecedLevel2(string op)
    {
        if (op == "*" || op == "/" || op == "%") return true;
        return false;
    }
    private static bool isPrecedLevel1(string op)
    {
        if (op == "+" || op == "-") return true;
        return false;
    }
    private static bool isOperator(string op)
    {
        for (int i = 0; i < _operators.Length; i++)
        {
            if (op == _operators[i].ToString())
            {
                return true;
            }
        }
        return false;
    }
    //support for postfix comparision expression conversion.//
    private static bool isLogicalOperator(string op)
    {

        if (op == "||" ||
            op == "&&")
            return true;
        return false;
    }
    private static bool isComparisionOperator(string op)
    {

        if (op == ">" ||
           op == "<" ||
           op == "=" ||
           op == "<=" ||
           op == ">=" ||
           op == "==" ||
           op == "!=")
            return true;
        return false;
    }

    public static void printICode()
    {
        if (_icode == null) return;
        for (int i = 0; i < _icode.Count; i++)
        {
            if (i < 10) Console.Write(i + " :");
            else Console.Write(i + ":");
            Console.WriteLine(_icode[i]);
        }
    }

    public static List<string> getICode() { return _icode; }
}

