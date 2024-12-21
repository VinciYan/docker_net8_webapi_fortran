using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using STLib.Json;

namespace docker_net8_webapi_fortran.Controllers;

[ApiController]
[Route("[controller]")]
public class FortranCalculateController : Controller
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<FortranCalculateController> _logger;

    public FortranCalculateController(ILogger<FortranCalculateController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 调用so计算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResponse> PostCal(PostCalInput postCalInput)
    {
        //string text = "";
        //using (StreamReader reader = new StreamReader("C:\\Users\\Administrator\\Desktop\\新建文件夹 (2)\\参数文件2024_07_01_15_03_43.json"))
        //{
        //    text = reader.ReadToEnd();
        //}
        //ParamClass pc = STJson.Deserialize<ParamClass>(text);

        //// 输入参数
        //float B = float.Parse(pc.Par[0].Data.ToString());			//矩形断面底宽,m		{0}
        //float H = float.Parse(pc.Par[1].Data.ToString()); ;			//水深,m			{1}
        //float Q = float.Parse(pc.Par[2].Data.ToString()); ;			//通过流量,m3/s		{2}

        //float A;		//矩形断面面积,m2		{3}
        //float FR;		//弗劳德数			{4}
        //int X;			//判断流态			{5}
        //float V;		//流速,m/s			{6}
        //float VW;		//波速,m/s			{7}

        //OPEN_CHANNEL_FLOW_REGIME(
        //// 输入参数
        //ref B,			//矩形断面底宽,m		{0}
        //ref H,			//水深,m			{1}
        //ref Q,			//通过流量,m3/s		{2}

        //out  A,		//矩形断面面积,m2		{3}
        //out  FR,		//弗劳德数			{4}
        //out  X,			//判断流态			{5}
        //out  V,		//流速,m/s			{6}
        //out  VW);		//波速,m/s			{7}

        //pc.Par[3].Data = A;
        //pc.Par[4].Data = FR;
        //pc.Par[5].Data = X;
        //pc.Par[6].Data = V;
        //pc.Par[7].Data = VW;
        //return new JsonResult(pc);

        try
        {
            double[] res = new double[1] { 0.0 };
            // {
            //     Console.WriteLine("\n测试平方计算:");
            //     double input = 3.14;

            //     try
            //     {
            //         // 为结果分配内存
            //         double[] result = new double[1] { 0.0 };

            //         using var caller = new DllCaller(
            //             "./libtest.so",
            //             "calculate_square_",
            //             new MarshalType(input, DataType.DT_DOUBLE),
            //             new MarshalType(result, DataType.DT_DOUBLE, true, true)  // 作为数组传递
            //         );

            //         Console.WriteLine($"输入: {input}");
            //         Console.WriteLine($"输出: {result[0]}");  // 直接使用数组中的值
            //         Console.WriteLine("平方计算成功!");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"平方计算失败: {ex.Message}");
            //     }
            // }
            // {
            //     Console.WriteLine("\n测试数组求和:");
            //     double[] array = { 1.0, 2.0, 3.0, 4.0, 5.0 };

            //     try
            //     {
            //         // 为结果分配内存
            //         double[] result = new double[1] { 0.0 };

            //         using var caller = new DllCaller(
            //             "./libtest.so",
            //             "array_sum_",
            //             new MarshalType(array, DataType.DT_DOUBLE, false, true),
            //             new MarshalType(array.Length, DataType.DT_INT),
            //             new MarshalType(result, DataType.DT_DOUBLE, true, true)  // 作为数组传递
            //         );

            //         Console.WriteLine($"输入数组: [{string.Join(", ", array)}]");
            //         Console.WriteLine($"数组和: {result[0]}");  // 直接使用数组中的值
            //         Console.WriteLine("数组求和成功!");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"数组求和失败: {ex.Message}");
            //     }
            // }

            ParamClass pc = STJson.Deserialize<ParamClass>(postCalInput.Text);
            int size = pc.Par.Count;
            MarshalType[] args = new MarshalType[size];
            for (int i = 0; i < size; i++)
            {
                int DataType = pc.Par[i].DataType;
                int ArrayType = pc.Par[i].ArrayType;
                int IsOut = pc.Par[i].IsOut;
                object Data = pc.Par[i].Data;
                if (null != Data)
                {
                    if (pc.Par[i].ArrayType == 1 && pc.Par[i].DataType == 2 && pc.Par[i].IsOut != 1)
                    {
                        if (pc.Par[i].Data is Object[])
                        {
                            double[] dData = ((object[])pc.Par[i].Data).Select(x => Convert.ToDouble(x)).ToArray();
                            args[i] = new MarshalType(dData, (DataType)DataType, IsOut == 1 ? true : false, ArrayType == 1 ? true : false);
                        }
                        continue;
                    }
                    args[i] = new MarshalType(Data, (DataType)DataType, IsOut == 1 ? true : false, ArrayType == 1 ? true : false);
                }
                else
                {
                    args[i] = new MarshalType(res, (DataType)DataType, IsOut == 1 ? true : false, ArrayType == 1 ? true : false);
                }

            }
            string libraryName = $"libtest.so";
            string path = Path.Combine(AppContext.BaseDirectory, "fortran", libraryName);
            // string path = $"C:\\Users\\Administrator\\Desktop\\新建文件夹 (2)\\WebApplication1\\WebApplication1\\bin\\Debug\\net8.0\\{pc.FuncName}.dll";
            // string path = $"/home/vinci/code/demo/docker_net8_webapi_fortran/bin/Debug/net8.0/test/libtest.so";            
            DllCaller dllCaller = new DllCaller(path, pc.FuncName, args);
            for (int i = 0; i < size; i++)
            {
                pc.Par[i].Data = args[i].GetResultValue();
            }
            return ApiResponse.OK(new JsonResult(pc));
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Error:{ex}");
        }

    }
}
