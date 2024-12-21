using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
public class DllCaller : IDisposable
{
    // Linux下动态库加载相关的导入函数
    [DllImport("libdl.so.2")]
    private static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so.2")]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so.2")]
    private static extern int dlclose(IntPtr handle);

    [DllImport("libdl.so.2")]
    private static extern IntPtr dlerror();

    // Linux下的动态库加载标志
    private const int RTLD_LAZY = 1;
    private const int RTLD_NOW = 2;
    private const int RTLD_GLOBAL = 8;

    private IntPtr libPtr;
    private MethodInfo method;

    public DllCaller(string soFile, string functionName, params MarshalType[] inargs)
    {
        if (soFile == null) throw new ArgumentNullException(nameof(soFile));
        if (functionName == null) throw new ArgumentNullException(nameof(functionName));

        // 加载.so文件
        this.libPtr = dlopen(soFile, RTLD_NOW);
        if (this.libPtr == IntPtr.Zero)
        {
            IntPtr errPtr = dlerror();
            string errorMessage = Marshal.PtrToStringAnsi(errPtr);
            throw new DllNotFoundException($"无法加载库文件 {soFile}: {errorMessage}");
        }

        // 获取函数地址
        IntPtr procPtr = dlsym(this.libPtr, functionName);
        if (procPtr == IntPtr.Zero)
        {
            IntPtr errPtr = dlerror();
            string errorMessage = Marshal.PtrToStringAnsi(errPtr);
            throw new EntryPointNotFoundException($"无法找到函数 {functionName}: {errorMessage}");
        }

        // 准备参数
        int arglength = inargs.Length;
        object[] args = new object[arglength];
        for (int i = 0; i < arglength; i++)
        {
            args[i] = inargs[i].GetInputValue();
        }

        // 动态创建方法
        AssemblyName asmName = new AssemblyName();
        asmName.Name = "DynamicAssembly";

        AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule("DynamicModule");

        Type resultType = typeof(void);
        Type[] argTypes = new Type[args.Length];
        for (int i = 0; i < args.Length; i++)
            argTypes[i] = args[i].GetType();

        MethodBuilder funBuilder = modBuilder.DefineGlobalMethod(
            functionName,
            MethodAttributes.Public | MethodAttributes.Static,
            resultType,
            argTypes
        );

        // 生成IL代码
        ILGenerator ilGen = funBuilder.GetILGenerator();
        for (int i = 0; i < args.Length; i++)
            ilGen.Emit(OpCodes.Ldarg, i);

        if (IntPtr.Size == 4)
            ilGen.Emit(OpCodes.Ldc_I4, (int)procPtr);
        else if (IntPtr.Size == 8)
            ilGen.Emit(OpCodes.Ldc_I8, (long)procPtr);

        // Linux下使用Cdecl调用约定
        ilGen.EmitCalli(OpCodes.Calli, CallingConvention.Cdecl, resultType, argTypes);
        ilGen.Emit(OpCodes.Ret);

        modBuilder.CreateGlobalFunctions();
        this.method = modBuilder.GetMethod(functionName);
        this.method.Invoke(null, args);

        // 调用函数后更新所有引用参数的值
        foreach (var arg in inargs)
        {
            arg.UpdateValue();
        }
    }

    public void Dispose()
    {
        if (this.method != null)
        {
            if (this.libPtr != IntPtr.Zero)
                dlclose(this.libPtr);
            this.libPtr = IntPtr.Zero;
            this.method = null;
            GC.SuppressFinalize(this);
        }
    }

    ~DllCaller()
    {
        this.Dispose();
    }
}
