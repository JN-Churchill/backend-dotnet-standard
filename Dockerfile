# 多阶段构建Dockerfile - 优化镜像大小
# Stage 1: 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件并还原依赖
COPY ["backendStd.Web.Entry/backendStd.Web.Entry.csproj", "backendStd.Web.Entry/"]
COPY ["backendStd.Application/backendStd.Application.csproj", "backendStd.Application/"]
COPY ["backendStd.Core/backendStd.Core.csproj", "backendStd.Core/"]
COPY ["backendStd.Common/backendStd.Common.csproj", "backendStd.Common/"]
COPY ["backendStd.Web.Core/backendStd.Web.Core.csproj", "backendStd.Web.Core/"]

RUN dotnet restore "backendStd.Web.Entry/backendStd.Web.Entry.csproj"

# 复制所有源代码并构建
COPY . .
WORKDIR "/src/backendStd.Web.Entry"
RUN dotnet build "backendStd.Web.Entry.csproj" -c Release -o /app/build

# Stage 2: 发布阶段
FROM build AS publish
RUN dotnet publish "backendStd.Web.Entry.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: 运行时阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安装时区数据和中文字体（可选）
RUN apt-get update && apt-get install -y \
    tzdata \
    && rm -rf /var/lib/apt/lists/*

# 设置时区为中国标准时间
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# 创建非root用户运行应用（安全最佳实践）
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# 从发布阶段复制文件
COPY --from=publish /app/publish .

# 暴露端口
EXPOSE 8080
EXPOSE 8081

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# 健康检查
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# 启动应用
ENTRYPOINT ["dotnet", "backendStd.Web.Entry.dll"]
