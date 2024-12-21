/// <summary>
/// 返回值统一封装
/// </summary>
public class ApiResponse
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public object Data { get; set; }

    public static ApiResponse OK()
    {
        ApiResponse apiResponse = new ApiResponse()
        {
            Success = true,
            Message = "Success"
        };
        return apiResponse;
    }

    public static ApiResponse OK(object data)
    {
        ApiResponse apiResponse = new ApiResponse()
        {
            Success = true,
            Message = "Success",
            Data = data
        };
        return apiResponse;
    }

    //public static ApiResponse OK(string message)
    //{
    //    ApiResponse apiResponse = new ApiResponse()
    //    {
    //        Success = true,
    //        Message = message
    //    };
    //    return apiResponse;
    //}

    public static ApiResponse OK(object data, string message)
    {
        ApiResponse apiResponse = new ApiResponse()
        {
            Success = true,
            Message = message,
            Data = data
        };
        return apiResponse;
    }

    public static ApiResponse Fail()
    {
        ApiResponse apiResponse = new ApiResponse()
        {
            Success = false,
            Message = "Fail"
        };
        return apiResponse;
    }

    public static ApiResponse Fail(string message)
    {
        ApiResponse apiResponse = new ApiResponse()
        {
            Success = false,
            Message = message
        };

        return apiResponse;
    }
}