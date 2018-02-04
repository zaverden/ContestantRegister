FROM microsoft/dotnet:2.0-sdk-jessie
WORKDIR /app

COPY _publish/ ./
EXPOSE 5000

ENTRYPOINT ["dotnet", "ContestantRegister.dll"]
