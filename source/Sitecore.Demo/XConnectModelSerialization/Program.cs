using System;
using System.IO;
using Sitecore.Demo.Model.XConnect.XConnectExtensions;
using Sitecore.XConnect.Serialization;

namespace XConnectModelSerialization
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Generating your model...");

            var model = CollectionModel.Model;

            var serializedModel = XdbModelWriter.Serialize(model);

            var pathToOutput = "c:\\temp\\";

            if (!Directory.Exists(pathToOutput)) Directory.CreateDirectory(pathToOutput);

            File.WriteAllText(pathToOutput + model.FullName + ".json", serializedModel);

            Console.WriteLine("Press any key to continue! Your model is here: " + "c:\\temp\\" + model.FullName +
                              ".json");
            Console.ReadKey();
        }
    }
}