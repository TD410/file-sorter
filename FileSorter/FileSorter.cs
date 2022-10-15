using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorter
{
	class FileSorter
	{
		static void Main(string[] args)
		{
			/*
			String mode = args[0];

			switch (mode) {
				case "sort":
					sort(args);
					break;
				case "detect":
					detect(args);
					break;
				default:
					Console.WriteLine("Wrong mode.");
					break;
			} */
			detect(args);
			Console.ReadLine();
		}

		// Sort file formats into folders
		static void sort(string[] args)
		{
			String inputFolder = args[1];
			String fileSignature = args[3];
			String outputFolder = Path.Combine(args[2], fileSignature);

			var filePaths = Directory.GetFiles(inputFolder, "*", SearchOption.AllDirectories).ToList();
			var matchFilePaths = new List<String>();

			foreach (var filePath in filePaths)
			{
				using (BinaryReader b = new BinaryReader(File.Open(filePath, FileMode.Open)))
				{
					var currentSignature = Encoding.GetEncoding("shift_jis").GetString(b.ReadBytes(fileSignature.Length));
					if (currentSignature == fileSignature)
					{
						matchFilePaths.Add(filePath);
					}
				}
			}

			Console.WriteLine("Number of {0} files: {1}.", fileSignature, matchFilePaths.Count);
			Console.WriteLine("Copying files...");

			Directory.CreateDirectory(outputFolder);
			foreach (var filePath in matchFilePaths)
			{
				File.Copy(filePath, Path.Combine(outputFolder, Path.GetFileName(filePath)), true);
			}

			Console.WriteLine("Done.");
		}

		// Detect file formats
		static void detect(string[] args)
		{
			String inputFolder = args[0];
			var filePaths = Directory.GetFiles(inputFolder, "*", SearchOption.AllDirectories).ToList();
			var fileSignatures = new Dictionary<String, int>();

			foreach (var filePath in filePaths)
			{
				using (BinaryReader b = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
				{
					var currentSignature = Encoding.GetEncoding("shift_jis").GetString(b.ReadBytes(16));
					currentSignature = currentSignature.Split('\0')[0];
					if (fileSignatures.ContainsKey(currentSignature))
					{
						fileSignatures[currentSignature] = fileSignatures[currentSignature] + 1;
					}
					else
					{
						Console.WriteLine("Found type: {0} - First file: {1}", currentSignature, Path.GetFileName(filePath));
						fileSignatures.Add(currentSignature, 1);
					}
				}
			}

			foreach (var entry in fileSignatures)
			{
				Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
			}
		}
	}
}
