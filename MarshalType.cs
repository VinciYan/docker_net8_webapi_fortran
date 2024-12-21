using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Xml.Linq;
public enum DataType
{
    //int 类型
    DT_INT,
    //float类型
    DT_FLOAT,
    //double类型
    DT_DOUBLE
}
public class MarshalType : IDisposable
{
    public IntPtr ptr;
    private MarshalType() { }
    private DataType _type;
    private bool _bArray;
    private object _data;
    private bool _bRef;
    private int[] _ArraySize;
    /// <summary>
    /// 数组长度
    /// </summary>
    private int _ArrayLength;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="data">输入数据</param>
    /// <param name="dt">数据类型</param>
    /// <param name="bRef">是否为值类型</param>
    /// <param name="bArray">是否数组</param>
    public MarshalType(object data, DataType dt, bool bRef = false, bool bArray = false)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _bRef = bRef;
        _type = dt;
        _bArray = bArray;

        if (_type == DataType.DT_INT)
        {
            if (bArray)
            {
                int[] narray = (int[])data;
                _ArrayLength = narray.Length;
                this.ptr = Marshal.AllocHGlobal(sizeof(int) * _ArrayLength);
                Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(int));
                int[] narray = new int[1];
                narray[0] = int.Parse(data.ToString());
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
        else if (_type == DataType.DT_FLOAT)
        {
            if (bArray)
            {
                float[] narray = (float[])data;
                _ArrayLength = narray.Length;
                this.ptr = Marshal.AllocHGlobal(sizeof(float) * _ArrayLength);
                Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(float));
                float[] narray = new float[1];
                narray[0] = float.Parse(data.ToString());
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
        else if (_type == DataType.DT_DOUBLE)
        {
            if (bArray)
            {
                if (data is double[])
                {
                    double[] narray = (double[])data;
                    _ArrayLength = narray.Length;
                    this.ptr = Marshal.AllocHGlobal(sizeof(double) * _ArrayLength);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
                else
                {
                    double[] narray = ((object[])data).Select(x => Convert.ToDouble(x)).ToArray();
                    _ArrayLength = narray.Length;
                    this.ptr = Marshal.AllocHGlobal(sizeof(double) * _ArrayLength);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(double));
                double[] narray = new double[1];
                narray[0] = double.Parse(data.ToString());
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="data">输入数据</param>
    /// <param name="dt">数据类型</param>
    /// <param name="bRef">是否为值类型</param>
    /// <param name="bArray">是否数组</param>
    /// <param name="ArraySize">多维数组每一维大小</param>
    public MarshalType(object data, DataType dt, bool bRef = false, bool bArray = false, params int[] ArraySize)
    {
        //_data = data;
        //_bRef = bRef;
        //_type = dt;
        //_bArray = bArray;
        //if (_type == DataType.DT_INT)
        //{
        //    if (bArray)
        //    {
        //        int[] narray = (int[])data;
        //        _ArrayLength = narray.Length;
        //        this.ptr = Marshal.AllocHGlobal(sizeof(int) * _ArrayLength);
        //        Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
        //    }
        //    else
        //    {
        //        this.ptr = Marshal.AllocHGlobal(sizeof(int));
        //        int[] narray = new int[1];
        //        narray[0] = (int)data;
        //        Marshal.Copy(narray, 0, this.ptr, 1);
        //    }
        //}
        //else if (_type == DataType.DT_FLOAT)
        //{
        //    if (bArray)
        //    {
        //        float[] narray = (float[])data;
        //        _ArrayLength = narray.Length;
        //        this.ptr = Marshal.AllocHGlobal(sizeof(float) * _ArrayLength);
        //        Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
        //    }
        //    else
        //    {
        //        this.ptr = Marshal.AllocHGlobal(sizeof(float));
        //        float[] narray = new float[1];
        //        narray[0] = (float)data;
        //        Marshal.Copy(narray, 0, this.ptr, 1);
        //    }
        //}
        //else if (_type == DataType.DT_DOUBLE)
        //{
        //    if (bArray)
        //    {
        //        double[] narray = (double[])data;
        //        _ArrayLength = narray.Length;
        //        this.ptr = Marshal.AllocHGlobal(sizeof(double) * _ArrayLength);
        //        Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
        //    }
        //    else
        //    {
        //        this.ptr = Marshal.AllocHGlobal(sizeof(double));
        //        double[] narray = new double[1];
        //        narray[0] = (double)data;
        //        Marshal.Copy(narray, 0, this.ptr, 1);
        //    }
        //}
        _data = data;
        _bRef = bRef;
        _type = dt;
        _bArray = bArray;
        _ArraySize = ArraySize;
        if (_type == DataType.DT_INT)
        {
            if (bArray)
            {
                if (ArraySize.Length == 0)
                {
                    int[] narray = (int[])data;
                    _ArrayLength = narray.Length;
                    this.ptr = Marshal.AllocHGlobal(sizeof(int) * _ArrayLength);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
                else
                {
                    _ArrayLength = GetArraySize(ArraySize);
                    this.ptr = Marshal.AllocHGlobal(sizeof(int) * _ArrayLength);
                    int[] narray = Change2OneDIntArray(data, ArraySize);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(int));
                int[] narray = new int[1];
                narray[0] = (int)data;
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
        else if (_type == DataType.DT_FLOAT)
        {
            if (bArray)
            {
                if (ArraySize.Length == 0)
                {
                    float[] narray = (float[])data;
                    _ArrayLength = narray.Length;
                    this.ptr = Marshal.AllocHGlobal(sizeof(float) * _ArrayLength);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
                else
                {
                    _ArrayLength = GetArraySize(ArraySize);
                    this.ptr = Marshal.AllocHGlobal(sizeof(float) * _ArrayLength);
                    float[] narray = Change2OneDFloatArray(data, ArraySize);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(float));
                float[] narray = new float[1];
                narray[0] = (float)data;
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
        else if (_type == DataType.DT_DOUBLE)
        {
            if (bArray)
            {
                if (ArraySize.Length == 0)
                {
                    double[] narray = (double[])data;
                    _ArrayLength = narray.Length;
                    this.ptr = Marshal.AllocHGlobal(sizeof(double) * _ArrayLength);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
                else
                {
                    _ArrayLength = GetArraySize(ArraySize);
                    this.ptr = Marshal.AllocHGlobal(sizeof(double) * _ArrayLength);
                    double[] narray = Change2OneDDoubleArray(data, ArraySize);
                    Marshal.Copy(narray, 0, this.ptr, _ArrayLength);
                }
            }
            else
            {
                this.ptr = Marshal.AllocHGlobal(sizeof(double));
                double[] narray = new double[1];
                narray[0] = (double)data;
                Marshal.Copy(narray, 0, this.ptr, 1);
            }
        }
    }

    /// <summary>
    /// 获取输入项值
    /// </summary>
    /// <returns></returns>
    // public object GetInputValue()
    // {
    //     if (_bRef)
    //         return ptr;
    //     else
    //         return _data;
    // }
    public object GetInputValue()
    {
        return _bRef ? ptr : _data;
    }
    public void UpdateValue()
    {
        if (_bRef && _type == DataType.DT_DOUBLE)
        {
            if (_bArray)
            {
                if (_ArraySize != null && _ArraySize.Length == 2)
                {
                    // 处理二维数组
                    double[] temp = new double[_ArrayLength];
                    Marshal.Copy(ptr, temp, 0, _ArrayLength);
                    double[,] matrix = (double[,])_data;

                    for (int i = 0; i < _ArraySize[0]; i++)
                    {
                        for (int j = 0; j < _ArraySize[1]; j++)
                        {
                            matrix[i, j] = temp[i * _ArraySize[1] + j];
                        }
                    }
                }
                else
                {
                    // 处理一维数组
                    var arr = (double[])_data;
                    Marshal.Copy(ptr, arr, 0, _ArrayLength);
                }
            }
            else
            {
                // 处理单个值
                double[] temp = new double[1];
                Marshal.Copy(ptr, temp, 0, 1);
                if (_data is double[] arr)
                {
                    arr[0] = temp[0];
                }
            }
        }
        if (_bRef && _type == DataType.DT_DOUBLE && _data is double[] array)
        {
            Marshal.Copy(ptr, array, 0, array.Length);
        }
    }
    /// <summary>
    /// 获取数值
    /// </summary>
    /// <returns></returns>
    public object GetResultValue()
    {
        unsafe
        {
            if (_bArray)
            {
                if (_type == DataType.DT_DOUBLE)
                {
                    double[] result = new double[_ArrayLength];
                    Marshal.Copy(ptr, result, 0, _ArrayLength);
                    return result;
                }
                if (_type == DataType.DT_INT)
                {
                    int[] ren = new int[_ArrayLength];
                    Marshal.Copy(this.ptr, ren, 0, _ArrayLength);
                    if (_ArraySize.Length == 0 || _ArraySize.Length == 1)
                        return ren;
                    else if (_ArraySize.Length == 2)
                    {
                        int[,] multiDArray = new int[_ArraySize[0], _ArraySize[1]];
                        return multiDArray;
                    }
                    else
                    {
                        throw new Exception("最大支持二维数组");
                    }
                }
                else if (_type == DataType.DT_FLOAT)
                {
                    float[] ren = new float[_ArrayLength];
                    Marshal.Copy(this.ptr, ren, 0, _ArrayLength);
                    if (_ArraySize.Length == 0 || _ArraySize.Length == 1)
                        return ren;
                    else if (_ArraySize.Length == 2)
                    {
                        float[,] multiDArray = new float[_ArraySize[0], _ArraySize[1]];
                        return multiDArray;
                    }
                    else
                    {
                        throw new Exception("最大支持二维数组");
                    }
                }
                else if (_type == DataType.DT_DOUBLE)
                {
                    double[] ren = new double[_ArrayLength];
                    Marshal.Copy(this.ptr, ren, 0, _ArrayLength);
                    if (_ArraySize.Length == 0 || _ArraySize.Length == 1)
                        return ren;
                    else if (_ArraySize.Length == 2)
                    {
                        double[,] multiDArray = new double[_ArraySize[0], _ArraySize[1]];
                        return multiDArray;
                    }
                    else
                    {
                        throw new Exception("最大支持二维数组");
                    }
                }
                else
                {
                    int[] ren = new int[_ArrayLength];
                    Marshal.Copy(this.ptr, ren, 0, _ArrayLength);
                    if (_ArraySize.Length == 0 || _ArraySize.Length == 1)
                        return ren;
                    else if (_ArraySize.Length == 2)
                    {
                        int[,] multiDArray = new int[_ArraySize[0], _ArraySize[1]];
                        return multiDArray;
                    }
                    else
                    {
                        throw new Exception("最大支持二维数组");
                    }
                }
            }
            else
            {
                if (_type == DataType.DT_DOUBLE)
                {
                    return Marshal.PtrToStructure<double>(ptr);
                }
                if (_type == DataType.DT_INT)
                    return *(int*)(ptr.ToPointer());
                else if (_type == DataType.DT_FLOAT)
                {
                    return *(float*)(ptr.ToPointer());
                }
                else if (_type == DataType.DT_DOUBLE)
                {
                    return *(double*)(ptr.ToPointer());
                }
                else
                    return *(int*)(ptr.ToPointer());
            }

        }
    }

    // public void Dispose()
    // {
    //     if (ptr != IntPtr.Zero)
    //     {
    //         Marshal.FreeHGlobal(ptr);
    //         this.ptr = IntPtr.Zero;
    //     }
    // }
    public void Dispose()
    {
        if (ptr != IntPtr.Zero)
        {
            if (_bRef)
            {
                UpdateValue();  // 在释放内存前更新值
            }
            Marshal.FreeHGlobal(ptr);
            ptr = IntPtr.Zero;
        }
    }
    ~MarshalType()
    {
        this.Dispose();
        GC.SuppressFinalize(this);
    }

    private int[] Change2OneDIntArray(object data, params int[] ArraySize)
    {
        if (ArraySize.Length == 1)
        {
            return (int[])data;
        }
        else if (ArraySize.Length == 2)
        {
            int[,] nArray = (int[,])data;

            //转化成一维数组
            int oneSize = ArraySize[0];
            int twoSize = ArraySize[1];
            int[] oneDimArray = new int[oneSize * twoSize];

            for (int i = 0; i < oneSize; i++)
            {
                for (int j = 0; j < twoSize; j++)
                {
                    int index = i * twoSize + j;
                    oneDimArray[index] = nArray[i, j];
                }
            }
            return oneDimArray;
        }
        else
        {
            throw new Exception("目前最大支持二维数组");
        }
    }

    private float[] Change2OneDFloatArray(object data, params int[] ArraySize)
    {
        if (ArraySize.Length == 1)
        {
            return (float[])data;
        }
        else if (ArraySize.Length == 2)
        {
            float[,] nArray = (float[,])data;

            //转化成一维数组
            int oneSize = ArraySize[0];
            int twoSize = ArraySize[1];
            float[] oneDimArray = new float[oneSize * twoSize];

            for (int i = 0; i < oneSize; i++)
            {
                for (int j = 0; j < twoSize; j++)
                {
                    int index = i * twoSize + j;
                    oneDimArray[index] = nArray[i, j];
                }
            }
            return oneDimArray;
        }
        else
        {
            throw new Exception("目前最大支持二维数组");
        }
    }

    private double[] Change2OneDDoubleArray(object data, params int[] ArraySize)
    {
        if (ArraySize.Length == 1)
        {
            return (double[])data;
        }
        else if (ArraySize.Length == 2)
        {
            double[,] nArray = (double[,])data;

            //转化成一维数组
            int oneSize = ArraySize[0];
            int twoSize = ArraySize[1];
            double[] oneDimArray = new double[oneSize * twoSize];

            for (int i = 0; i < oneSize; i++)
            {
                for (int j = 0; j < twoSize; j++)
                {
                    int index = i * twoSize + j;
                    oneDimArray[index] = nArray[i, j];
                }
            }
            return oneDimArray;
        }
        else
        {
            throw new Exception("目前最大支持二维数组");
        }
    }
    private int GetArraySize(params int[] ArraySize)
    {
        int size = 1;
        foreach (var r in ArraySize)
        {
            size = size * r;
        }
        return size;
    }
}
