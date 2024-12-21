public class ParamClass
{
    public string FuncName { get; set; }
    public string ClassName { get; set; }
    public List<Param> Par { get; set; }
}
public class Param
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 0.int 1.float 2.double
    /// </summary>
    public int DataType { get; set; }
    /// <summary>
    /// 是否为数组0非数组 1数组
    /// </summary>
    public int ArrayType { get; set; }
    /// <summary>
    /// 是否为输出参数 0输入参数 1输出参数
    /// </summary>
    public int IsOut { get; set; }
    /// <summary>
    /// 输入输出参数
    /// </summary>
    public object Data { get; set; }
}