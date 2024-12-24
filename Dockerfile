# ===== 第一阶段：构建阶段 =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 安装 Fortran 编译器
RUN apt-get update && apt-get install -y \
    gfortran \
    && rm -rf /var/lib/apt/lists/*

# 复制 Fortran 源文件并编译
COPY fortran/test.f90 /src/fortran/
WORKDIR /src/fortran
RUN gfortran -shared -fPIC -o libtest.so test.f90

# 回到主工作目录
WORKDIR /src

# 只复制项目文件
COPY ["docker_net8_webapi_fortran.csproj", "./"]
RUN dotnet restore

# 复制源代码
COPY . .

# 合并 build 和 publish 命令
RUN dotnet publish -c Release -o /app/publish

# 复制编译好的 .so 文件到发布目录
RUN mkdir -p /app/publish/fortran && \
    cp /src/fortran/libtest.so /app/publish/fortran/

# ===== 第二阶段：运行阶段 =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# 创建并配置 Debian 镜像源
RUN echo "deb https://mirrors.ustc.edu.cn/debian/ bookworm main contrib non-free non-free-firmware" > /etc/apt/sources.list && \
    echo "deb https://mirrors.ustc.edu.cn/debian/ bookworm-updates main contrib non-free non-free-firmware" >> /etc/apt/sources.list && \
    echo "deb https://mirrors.ustc.edu.cn/debian-security bookworm-security main contrib non-free non-free-firmware" >> /etc/apt/sources.list

# 安装运行时依赖
RUN apt-get update && apt-get install -y \
    libc6-dev \
    libgfortran5 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# 确保 fortran 目录和 .so 文件的权限正确
RUN chmod -R 755 /app/fortran && \
    chmod 755 /app/fortran/libtest.so

# 设置 LD_LIBRARY_PATH
ENV LD_LIBRARY_PATH=/app/fortran

# 设置端口和监听地址
# ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 5000

ENTRYPOINT ["dotnet", "docker_net8_webapi_fortran.dll"]
