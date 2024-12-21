# ===== 第一阶段：构建阶段 =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 只复制项目文件
COPY ["docker_net8_webapi_fortran.csproj", "./"]
RUN dotnet restore

# 复制源代码
COPY . .

# 合并 build 和 publish 命令
RUN dotnet publish -c Release -o /app/publish

# ===== 第二阶段：运行阶段 =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# 创建并配置 Debian 镜像源
RUN echo "deb https://mirrors.ustc.edu.cn/debian/ bookworm main contrib non-free non-free-firmware" > /etc/apt/sources.list && \
    echo "deb https://mirrors.ustc.edu.cn/debian/ bookworm-updates main contrib non-free non-free-firmware" >> /etc/apt/sources.list && \
    echo "deb https://mirrors.ustc.edu.cn/debian-security bookworm-security main contrib non-free non-free-firmware" >> /etc/apt/sources.list

# 安装依赖
RUN apt-get update && apt-get install -y \
    libc6-dev \
    build-essential \
    gfortran \
    libgfortran5 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# 确保 fortran 目录存在
RUN mkdir -p fortran

# 显式复制 so 文件
COPY fortran/libtest.so /app/fortran/
RUN chmod +x /app/fortran/libtest.so

# 修复 LD_LIBRARY_PATH 的设置
ENV LD_LIBRARY_PATH=/app/fortran

EXPOSE 5000
# ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "docker_net8_webapi_fortran.dll"]