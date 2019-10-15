using System;
using System.IO;
using Sitecore.Demo.Model.XConnect.XConnectExtensions;
using Sitecore.XConnect.Serialization;

namespace XConnectModelSerialization
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Generating your model...");

            var model = CollectionModel.Model;

            var serializedModel = XdbModelWriter.Serialize(model);

            var pathToOutput = AppContext.BaseDirectory;

            if (!Directory.Exists(pathToOutput)) Directory.CreateDirectory(pathToOutput);

            var modelFullName = pathToOutput + "LetsPlay.XConnectModel, 1.0" + ".json";
            File.WriteAllText(modelFullName, serializedModel);

            
        }
    }
}