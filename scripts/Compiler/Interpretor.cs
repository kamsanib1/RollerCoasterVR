using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//defines the type of data of variable.//
//
public enum Type { INT, FLOAT, INT_ARRAY_1D, FLOAT_ARRAY_1D, INT_ARRAY_2D, FLOAT_ARRAY_2D }

//class to create array data structure.//
//usage://
//use constructor to initiate type of array//
//then initialize must be performed to overcome exceptions.//
//always update data using row and col index(*even the first assignment.)// 
//0-based indexing system//
public class Array<T>
{
    string _name;
    bool _2d = false;
    int _rows = 1;
    int _cols = 1;
    T[] _values;

    //variable//
    public Array(string name) : this(name, false, 1, 1) { }
    //1D array//
    public Array(string name, int length) : this(name, false, length, 1) { }
    //2D array//
    public Array(string name, int rows, int cols) : this(name, true, rows, cols) { }
    //main constructor//
    public Array(string name, bool is2d, int rows, int cols) { _name = name; _2d = is2d; _rows = rows; _cols = cols; _values = new T[rows * cols]; }

    public string getName() { return _name; }
    public bool isInRange(int index)
    {
        if (index < _rows && index >= 0)
            return true;
        return false;
    }
    public bool isInRange(int row, int col)
    {
        if (row < _rows && row >= 0 && col >= 0 && col < _cols)
            return true;
        return false;
    }

    //set value for any array//
    public bool setValue(int index, T value)
    {
        if (index >= _values.Length) return false;// default(T);
        _values[index] = value;
        return true;// _values[index];
    }
    //set value strictly for 2d array//
    public bool setValue(int row, int col, T value)
    {
        if (!_2d) return false;
        return setValue(row * _cols + col, value);
    }

    //get value for any array//
    public T getValue(int index)
    {
        if (index >= _values.Length) return default(T);
        return _values[index];
    }
    //get value strictly for 2d array//
    public T getValue(int row, int col)
    {
        if (!_2d) return default(T);
        //Console.WriteLine("debug:{0}:value at index({1},{2}) is {3}", _name, row,col, _values[row*_cols+col]);
        return getValue(row * _cols + col);
    }
}

public class DataType
{
    string name;
    double value;
    private Type _type;

    public DataType(string __name, float __value, Type __type) { name = __name; _type = __type; setValue(__value); }

    //public void setType(Type __type) { type = __type; }
    public void setValue(double __value) { value = __value; }
    public Type getType() { return _type; }
    public string getName() { return name; }
    public double getValue() { return value; }
}

public class Interpretor
{
    private List<String> _icode;
    private List<string[]> _code;
    private int _pointer;
    private List<DataType> variables;
    private List<Array<int>> _intArray;
    private List<Array<double>> _floatArray;
    private string _error;
    private bool _errorFlag = false;
    Random rand = new Random();

    //runtime variables updated by parent scripts.//
    public bool collision;
    public bool shot;

    public Interpretor() { }
    public Interpretor(List<string> __intermediateCode) { _icode = __intermediateCode; }
    public void setICode(List<string> __intermediateCode) { _icode = __intermediateCode; }

    public void init()
    {
        _pointer = 0;
        _code = new List<string[]>();

        //variables and data.
        variables = new List<DataType>();
        _intArray = new List<Array<int>>();
        _floatArray = new List<Array<double>>();

        if (_icode != null)
        {
            for (int i = 0; i < _icode.Count; i++)
            {
                string[] __tmp = _icode[i].Split(' ');
                _code.Add(__tmp);
            }
        }
    }
    public string[] nextIns()
    {
        string[] res = null;

        while (true)
        {
            string[] __stat = splitWithWhitesplace( _icode[_pointer]);
            if (__stat[0] == "var") { variable(__stat); }
            else if (__stat[0] == "arithmatic") { arithmatic(__stat); }
            else if (__stat[0] == "comparision") { comparision(__stat); }
            else if (__stat[0] == "if") { ifStat(__stat); }
            else if (__stat[0] == "branch") { branch(__stat); }
            else if (__stat[0] == "function")
            {
                if (Library.isStdFunc(__stat[1])) { stdFunction(__stat); }
                else
                {
                    res = function(__stat);
                    break;
                }
            }
            else if (__stat[0] == Compiler.EOP) { res = new string[1]; res[0] = Compiler.EOP; break; }
        }
        if (_errorFlag) res[0] = Compiler.EOP;
        return res;
    }
    private void comparision(string[] __stat)
    {
        double res = 0;
        string var = __stat[1];
        double lhs = getValue(__stat[2]);
        double rhs = getValue(__stat[3]);
        string op = __stat[4];

        if (op == ">") { if (lhs > rhs) res = 1; }
        else if (op == ">=") { if (lhs >= rhs) res = 1; }
        else if (op == "<") { if (lhs < rhs) res = 1; }
        else if (op == "<=") { if (lhs <= rhs) res = 1; }
        else if (op == "==") { if (lhs == rhs) res = 1; }
        else if (op == "=") { if (lhs == rhs) res = 1; }
        else if (op == "!=") { if (lhs != rhs) res = 1; }
        else if (op == "||") { if (lhs != 0 || rhs != 0) res = 1; }
        else if (op == "&&") { if (lhs != 0 && rhs != 0) res = 1; }
        else if (op == "!")  { if (lhs == 0) res = 1; }

        setValue(var, res);
        _pointer++;
    }
    private void ifStat(string[] __stat)
    {
        double lhs = getValue(__stat[1]);
        if (lhs == 1) _pointer++;
        _pointer++;
    }
    private string[] function(string[] __stat)
    {
        string[] res = new string[__stat.Length - 1];
        res[0] = __stat[1];
        string[] args = Library.getArgs(__stat[1]).Split(':');
        for (int i = 2; i < __stat.Length; i++)
        {
            if (__stat[i] == "") continue;
            if (isDirection(__stat[i]))
                res[i - 1] = __stat[i];
            else if (isTunnel(__stat[i]))
                res[i - 1] = __stat[i];
            else if (isString(__stat[i])) { res[i - 1] = __stat[i]; }
            else if (__stat[1] == "place" && i == 2) { res[i - 1] = __stat[i]; }
            //else if (isArrayNameF(__stat[i])) { res[i - 1] = __stat[i]; }
            else if (args[i - 2] == "var" || args[i - 2] == "string2")
                res[i - 1] = __stat[i];
            else 
                res[i - 1] = getValue(__stat[i]).ToString();
        }
        //tmp workaround for raycast function.//
        if (__stat[1] == "raycast" || __stat[1] == "collidercoords" || __stat[1] == "collidertype" || __stat[1] == "colliderangle")
        {
            res[__stat.Length - 2] = __stat[__stat.Length - 1];
        }
        //tmp workaround for raycast function.//
        _pointer++;
        return res;
    }
    private void arithmatic(string[] __stat)
    {
        double left = getValue(__stat[2]);
        double right = getValue(__stat[3]);
        char op = __stat[4][0];
        double lhs = 0;

        switch (op)
        {
            case '+': lhs = left + right; break;
            case '-': lhs = left - right; break;
            case '*': lhs = left * right; break;
            case '/':
                if (right == 0) break;
                lhs = left / right;
                break;
            case '%':
                if (right == 0) break;
                lhs = left % right;
                break;
            case '^':
                lhs = Math.Pow(left, right);
                break;
        }
        setValue(__stat[1], lhs);
        _pointer++;
    }
    private void branch(string[] __stat)
    {
        _pointer = int.Parse(__stat[1]);
    }
    private void variable(string[] __stat)
    {
        bool __flag = true;

        for (int i = 0; i < variables.Count; i++)
        {
            if (__stat[2] == variables[i].getName())
            {
                __flag = false;
                break;
            }
        }

        if (__flag) { createVariable(__stat); }
        _pointer++;
    }
    private void stdFunction(string[] __stat)
    {
        string[] args = Library.getArgs().Split(':');
        if (__stat[1] == "random")
        {
            double start = getValue(__stat[3]);
            double end = getValue(__stat[4]);
            double num = rand.Next() % (int)end + start + rand.NextDouble();
            if (num > end) num = end;
            Data.output += start + "," + end + "," + num + "\n";
            //for (int i = 0; i < variables.Count; i++)
            //{
            //    //if (variables[i].getName() == __stat[2]) variables[i].setValue(num);
            //}
            setValue(__stat[2], num);
        }
        else if (__stat[1] == "display")
        {
            string var = __stat[2];
            if (var.Contains("("))
            {
                var = var.Replace(")", "");
                string[] tmp = var.Split('(');
                string row = tmp[1];
                string col = "";
                if (row.Contains(",")) { string[] tmp2 = row.Split(','); row = tmp2[0]; col = tmp2[1]; }
                var = tmp[0] + "(" + getValue(row);
                if (col != "") { var += "," + getValue(col); }
                var += ")";
            }

            Data.output += var + ":" + getValue(__stat[2]) + "\n";
        }
        else if (__stat[1] == "print")
        {
            string res = "";
            for (int i = 2; i < __stat.Length; i++) { res += __stat[i]; if (i != __stat.Length - 1) res += " "; }
            res = res.Replace("\"", "");
            string[] tokens = res.Split('+');
            res = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                if (isVariable(tokens[i]))
                    res += getValue(tokens[i]);
                else res += tokens[i];
            }
            Data.output += res + "\n";
        }
        else if (__stat[1] == "load")
        {
            string path = __stat[2].Replace("\"", "");
            string tmp = FileManager.loadFile(path);
            string[] code = tmp.Split('\n');
            for (int i = 0; i < _floatArray.Count; i++)
            {
                if (_floatArray[i].getName() == __stat[3])
                {
                    string[] tmp2 = code[0].Split(' ');
                    int row = int.Parse(tmp2[0]);
                    int col = int.Parse(tmp2[1]);
                    if (_floatArray[i].isInRange(row - 1, col - 1))
                    {
                        for (int j = 1; j <= row; j++)
                        {
                            tmp2 = code[j].Split(' ');
                            for (int k = 1; k <= col; k++)
                            {
                                double val = double.Parse(tmp2[k - 1]);
                                _floatArray[i].setValue(j - 1, k - 1, val);
                            }
                        }
                    }
                    else Data.output += "Runtime error:failed to load file '" + __stat[2] + "'." + "row column mismatch for array with file.";
                }
            }
        }
        else if (__stat[1] == "collision")
        {
            if (collision == true)
            {
                setValue(__stat[2], 1);
            }
            else
            {
                setValue(__stat[2], 0);
            }
        }
        else if (__stat[1] == "loadscene") { loadScene(__stat[2]); }
        else if (__stat[1] == "placeobject") { placeDObject(__stat[2], __stat[3], __stat[4], __stat[5], __stat[6]); }
        else if (__stat[1] == "playsound") { playAudio(__stat[2], "once"); }
        else if (__stat[1] == "getposition") { position(__stat[2], __stat[3], __stat[4], __stat[5]); }
        else if (__stat[1] == "gotshot") { gotShot(__stat[2]); }
        else if (__stat[1] == "destroy") { destroyObject(__stat[2]); }
        else if(__stat[1] == "getglobal") { globalVarHandler(__stat[2],__stat[3],true); }
        else if (__stat[1] == "setglobal") { globalVarHandler(__stat[2], __stat[3], false); }
        else if(__stat[1] == "debug") { UnityEngine.Debug.Log("(game variable)"+__stat[2]+":"+getValue(__stat[2])); }
        _pointer++;
    }

    private string[] splitWithWhitesplace(string str)
    {
        List<string> list = new List<string>();
        string[] output;

        bool flag = false;
        string target = "";
        foreach(char ch in str)
        {
            if (ch == ' ' && !flag)
            {
                list.Add(target);
                target = "";
            }
            else if (ch == '"')
            {
                flag = !flag;
            }
            else target += ch;
        }
        if (target != "") list.Add(target);

        output = new string[list.Count];
        for(int i = 0; i < list.Count; i++) { output[i] = list[i]; }
        return output;
    }
    public double getValue(string __var)
    {
        //extracting variable data in case of array.//
        int row = -1;
        int col = -1;
        double mathParam = 0;
        //Console.WriteLine("Debug:var({0})", __var);

        if (__var.Contains("("))
        {
            __var = __var.Replace(")", "");
            string[] tmp = __var.Split('(');
            string[] loc = tmp[1].Split(',');
            __var = tmp[0];

            mathParam = getValue(loc[0]);
            row = (int)mathParam;
            if (loc.Length == 2) col = (int)getValue(loc[1]);
        }

        //checking for math functions//
        if (Library.isMathFunc(__var))
        {
            return Library.runFunction(__var, mathParam);
        }

        //-1 is added to be consistent between 1-based to 0-based indexing//
        row--;
        col--;
        //retrive value from variable.//
        for (int i = 0; i < variables.Count; i++)
        {
            if (variables[i].getName() == __var)
            {
                double value = 0;

                switch (variables[i].getType())
                {
                    case Type.INT:
                    case Type.FLOAT:
                        value = variables[i].getValue();
                        break;
                    case Type.FLOAT_ARRAY_1D:
                    case Type.FLOAT_ARRAY_2D:
                        for (int j = 0; j < _floatArray.Count; j++)
                        {
                            if (_floatArray[j].getName() == __var)
                            {
                                if (col < 0)
                                {
                                    if (_floatArray[j].isInRange(row))
                                        value = _floatArray[j].getValue(row);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1); }
                                }
                                else {
                                    if (_floatArray[j].isInRange(row, col))
                                        value = _floatArray[j].getValue(row, col);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1) + "," + (col + 1); }
                                }
                            }
                        }
                        break;
                    case Type.INT_ARRAY_1D:
                    case Type.INT_ARRAY_2D:
                        for (int j = 0; j < _intArray.Count; j++)
                        {
                            if (_intArray[j].getName() == __var)
                            {
                                if (col < 0)
                                {
                                    if (_intArray[j].isInRange(row))
                                        value = _intArray[j].getValue(row);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1); }
                                }
                                else {
                                    if (_intArray[j].isInRange(row, col))
                                        value = _intArray[j].getValue(row, col);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1) + "," + (col + 1); }
                                }
                            }
                        }
                        break;
                }
                return value;
            }
        }
        //UnityEngine.Debug.Log(__var);
        return double.Parse(__var);
    }
    public void setValue(string __var, double __value)
    {

        //extracting variable data in case of array.//
        int row = -1;
        int col = -1;
        //hack to handle temporary variables.//
        if (__var.Contains("$")) { string[] tmp = {"","float",__var }; variable(tmp);_pointer--; }
        if (__var.Contains("("))
        {
            __var = __var.Replace(")", "");
            string[] tmp = __var.Split('(');
            string[] loc = tmp[1].Split(',');
            __var = tmp[0];
            //-1 is added to be consistent between 1-based to 0-based indexing//
            row = (int)getValue(loc[0]) - 1;
            if (loc.Length == 2) col = (int)getValue(loc[1]) - 1;
        }

        for (int i = 0; i < variables.Count; i++)
        {
            if (variables[i].getName() == __var)
            {

                //double value = 0;
                switch (variables[i].getType())
                {
                    case Type.INT:
                        variables[i].setValue((int)__value);
                        break;
                    case Type.FLOAT:
                        variables[i].setValue(__value);
                        break;
                    case Type.FLOAT_ARRAY_1D:
                    case Type.FLOAT_ARRAY_2D:
                        for (int j = 0; j < _floatArray.Count; j++)
                        {
                            if (_floatArray[j].getName() == __var)
                            {
                                if (col < 0)
                                {
                                    if (_floatArray[j].isInRange(row))
                                        _floatArray[j].setValue(row, __value);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1); }
                                }
                                else
                                {
                                    if (_floatArray[j].isInRange(row, col))
                                        _floatArray[j].setValue(row, col, __value);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1) + "," + (col + 1); }
                                }
                            }
                        }
                        break;
                    case Type.INT_ARRAY_1D:
                    case Type.INT_ARRAY_2D:
                        for (int j = 0; j < _intArray.Count; j++)
                        {
                            if (_intArray[j].getName() == __var)
                            {
                                if (col < 0)
                                {
                                    if (_intArray[j].isInRange(row))
                                        _intArray[j].setValue(row, (int)__value);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1); }
                                }
                                else
                                {
                                    if (_intArray[j].isInRange(row, col))
                                        _intArray[j].setValue(row, col, (int)__value);
                                    else { _errorFlag = true; _error = __var + ":index out of range:" + (row + 1) + "," + (col + 1); }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
    private void createVariable(string[] __stat)
    {
        string type = __stat[1];
        string variable = __stat[2];
        variable = variable.Replace(")", "");
        string[] tmp = variable.Split('(');
        Type typeL = Type.INT;

        //case:variable
        if (tmp.Length == 1)
        {
            if (type == "int") typeL = Type.INT;
            else if (type == "float") typeL = Type.FLOAT;
        }
        //case:array
        else
        {
            string[] args = tmp[1].Split(',');
            //case:1D array
            if (args.Length == 1)
            {
                //case:1D integer array
                if (type == "int")
                {
                    typeL = Type.INT_ARRAY_1D;
                    int size = (int)getValue(args[0]);
                    Array<int> local = new Array<int>(tmp[0], size);
                    //add 1D array
                    _intArray.Add(local);
                }
                //case:1D floating points array
                else if (type == "float")
                {
                    typeL = Type.FLOAT_ARRAY_1D;
                    int size = (int)getValue(args[0]);
                    Array<double> local = new Array<double>(tmp[0], size);
                    //add 1D array
                    _floatArray.Add(local);
                }

            }
            //case:2D array
            else if (args.Length == 2)
            {
                //case:2D integer array
                if (type == "int")
                {
                    typeL = Type.INT_ARRAY_2D;
                    int rows = (int)getValue(args[0]);
                    int cols = (int)getValue(args[1]);
                    Array<int> local = new Array<int>(tmp[0], rows, cols);
                    //add 2D array
                    _intArray.Add(local);
                }
                //case:2D floating point array
                else if (type == "float")
                {
                    typeL = Type.FLOAT_ARRAY_2D;
                    int rows = (int)getValue(args[0]);
                    int cols = (int)getValue(args[1]);
                    Array<double> local = new Array<double>(tmp[0], rows, cols);
                    //add 2D array
                    _floatArray.Add(local);
                }
            }
        }
        //add new variable to the list of variables. 
        variables.Add(new DataType(tmp[0], 0, typeL));
    }
    public void printError()
    {
        if (_errorFlag) Console.WriteLine("Runtime Error:" + _error);
    }

    private bool isDirection(string __var)
    {
        bool flag = false;
        if (__var.ToLower() == "left") flag = true;
        else if (__var.ToLower() == "right") flag = true;
        else if (__var.ToLower() == "up") flag = true;
        else if (__var.ToLower() == "down") flag = true;
        else if (__var.ToLower() == "forward") flag = true;
        else if (__var.ToLower() == "backward") flag = true;
        return flag;
    }
    //rollercoaster specific function//
    private bool isTunnel(string __var)
    {
        if (__var == "start" || __var == "end") return true;
        return false;
    }
    private bool isString(string str)
    {
        if (str[0] == '"' && str[str.Length - 1] == '"') { return true; }
        return false;
    }
    private bool isArrayNameF(string str)
    {
        for (int i = 0; i < _floatArray.Count; i++)
        {
            if (_floatArray[i].getName() == str) return true;
        }
        return false;
    }

    private bool isVariable(string var) {
        string[] tmp = var.Split('(');
        for(int i = 0; i < variables.Count; i++) {
            if (tmp[0] == variables[i].getName())
                return true;
        }
        return false;
    }
    public void setValueParameter(string __var, double __value)
    {
        setValue(__var,__value);
    }

    //unity independent functions.//
    void gotShot(string var)
    {
        double val = 0;
        if (shot) val = 1;
        //UnityEngine.Debug.Log("shot:" + val);
        shot = false;
        setValue(var, val);
    }
    //unity dependent functions//
    void loadScene(string sceneName)
    {
        Data.loadScene(sceneName);
    }

    //places an object mentioned within files section.//
    void placeDObject(string objName, string name, string x, string y, string z) {
        UnityEngine.Debug.Log("placing object...");
        objName = objName.Replace("\"", "");
        name = name.Replace("\"", "");
        //check for availability of object(objName).
        int index=-1;
        for (int i = 0; i < Data.files.Count; i++) if(Data.files[i].rcname == objName) { index = i; break; }
        if (index < 0) {UnityEngine.Debug.Log(objName+" does not exist!!!"); return; }

        //check for redundant names.
        string suffix = "";
        for(int i=0,count = 0;i<Data.dObjects.Count;i++) if(Data.dObjects[i].rcname == name + suffix) { UnityEngine.Debug.Log(name+" already exists!!!"); return; count++; suffix = count.ToString(); i = 0; }
        name += suffix;

        //instantiate a new object and push it in stack.//
        float _x = float.Parse(x);
        float _y = float.Parse(y);
        float _z = float.Parse(z);

        UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(Data.files[index].file);
        obj.transform.position = new UnityEngine.Vector3(_x, _y, _z);
        DynamicObjects dobj = new DynamicObjects();
        dobj.rcname = name;
        dobj.gameObject = obj;
        Data.dObjects.Add(dobj);

    }

    void destroyObject(string objName)
    {
        int index = -1;
        for (int i = 0; i < Data.dObjects.Count; i++) if (Data.dObjects[i].rcname == objName) { index = i; UnityEngine.Debug.Log(Data.dObjects[i].rcname); break; }
        UnityEngine.GameObject.Destroy(Data.dObjects[index].gameObject);
        Data.dObjects.RemoveAt(index);
    }
    //plays an audio clip based on the options mentioned.//
    void playAudio(string name, string option)
    {
        //name and option verification.//
        name = name.Replace("\"", "");
        int index = -1;
        for (int i = 0; i < Data.data.sounds.Length; i++) if (Data.data.sounds[i].rcname == name) { index = i; break; }
        if (index < 0) { UnityEngine.Debug.Log(name + " audio clip not found!!!"); return; }
        if (option != "once" && option != "repeat") { UnityEngine.Debug.Log("invalid option " + option + "!!!"); return; }

        //creating a gameobject to play audio and destroying after finished playing.//
        UnityEngine.GameObject audio = new UnityEngine.GameObject("audio player");
        UnityEngine.AudioSource src = audio.AddComponent<UnityEngine.AudioSource>();
        if (option == "once") {
            src.PlayOneShot(Data.data.sounds[index].clip);
            DestroyScript ds = audio.AddComponent<DestroyScript>();
            ds.setTime(Data.data.sounds[index].clip.length+5);
        }
        else
        {
            src.clip = Data.data.sounds[index].clip;
            src.Play();
            src.loop = true;
        }
    }

    void position(string name, string xPos, string yPos, string zPos)
    {
        for(int i = 0; i < Data.objects.Count; i++)
        {
            MainObject obj = Data.objects[i].GetComponent<MainObject>();
            if (obj.nameO == name)
            {
                setValue(xPos, obj.gameObject.transform.position.x);
                setValue(yPos, obj.gameObject.transform.position.y);
                setValue(zPos, obj.gameObject.transform.position.z);
            }
        }
    }
    void globalVarHandler(string name, string val, bool isget)
    {
        name = name.Replace("\"", "");
        UnityEngine.Debug.Log("global handler.");
        Globals var = null;
        for (int i = 0; i < Data.globals.Count; i++)
        {
            if (Data.globals[i].rcname == name)
                var = Data.globals[i];
        }
        if (var == null) { var = new Globals(name, 0); Data.globals.Add(var); UnityEngine.Debug.Log("created global var:" + name); }
        UnityEngine.Debug.Log("global data:"+var.rcname + ":" + var.val);
        if (isget)
        {
            setValue(val, var.val);
            UnityEngine.Debug.Log("getting value:"+val+":"+var.rcname + ":" + var.val);
        }
        else
        {
            var.val = getValue(val);
        }
    }
}