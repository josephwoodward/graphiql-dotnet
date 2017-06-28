rm -rf ./package
cd ./src/graphiql/
dotnet pack ./graphiql.csproj -o ../../package/ -c release