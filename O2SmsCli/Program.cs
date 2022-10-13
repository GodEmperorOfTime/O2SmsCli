using O2SmsCli.Cli;
using O2SmsCli.Configuration;

var config = ConfigReader.ReadConfig<Program>(args);

var cli = new CliInterface(config);
await cli.RunInteractiveAsync();

Console.WriteLine("Ukonceno!");
