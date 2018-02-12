FROM microsoft/dotnet:2.0-sdk-jessie
WORKDIR /app

COPY _publish/ ./
RUN mkdir ./logs
VOLUME ./logs
EXPOSE 5000

ENTRYPOINT ["dotnet", "ContestantRegister.dll"]
