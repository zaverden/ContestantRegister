FROM microsoft/dotnet:2.0-sdk-jessie
WORKDIR /app

COPY _publish/ ./
RUN mkdir ./logs
VOLUME ./logs
VOLUME ./data-protection-keys
EXPOSE 5000

ENTRYPOINT ["dotnet", "ContestantRegister.dll"]
