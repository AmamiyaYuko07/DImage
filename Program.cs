using DImage;
using System.CommandLine;
using System.IO.Compression;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var rootCommand = new RootCommand("Compress/UnCompress File On Png Image");

var decodeCommand = new Command("d", "UnCompress File On Png Image");
var imageOption = new Option<string>(name: "--image", description: "Png Image File") { IsRequired = true };
imageOption.AddAlias("-i");
var encodeOption = new Option<string>(name: "--encode", description: "FileName Encode", getDefaultValue: () => "UTF-8");
encodeOption.AddAlias("-e");
var outputOption = new Option<string>(name: "--output", description: "Output File", getDefaultValue: () => "outputs");
outputOption.AddAlias("-o");
var filesOption = new Option<string[]>(name: "--file", description: "Target File") { IsRequired = true };
filesOption.AddAlias("-f");
var passwordOption = new Option<string>(name: "--password", description: "Password");
passwordOption.AddAlias("-p");
decodeCommand.AddOption(imageOption);
decodeCommand.AddOption(encodeOption);
decodeCommand.AddOption(outputOption);
decodeCommand.AddOption(passwordOption);
decodeCommand.SetHandler((image, encode, output) =>
{
    if (!File.Exists(image))
    {
        Console.WriteLine("Png file not found！");
        return;
    }
    ImageHelper.DImage(image, Encoding.GetEncoding(encode), output);
}, imageOption, encodeOption, outputOption);

var encodeCommand = new Command("e", "Compress File Into Png Image");
encodeCommand.AddOption(imageOption);
encodeCommand.AddOption(filesOption);
encodeCommand.AddOption(encodeOption);
encodeCommand.AddOption(outputOption);
encodeCommand.AddOption(passwordOption);
encodeCommand.SetHandler((image, files, output, encode) =>
{
    if (!File.Exists(image))
    {
        Console.WriteLine("Png file not found！");
        return;
    }
    foreach (var file in files)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine($"file:{file} not found！");
            return;
        }
    }
    ImageHelper.EImage(image, files, output, Encoding.GetEncoding(encode));
}, imageOption, filesOption, outputOption, encodeOption);
rootCommand.Add(decodeCommand);
rootCommand.Add(encodeCommand);
await rootCommand.InvokeAsync(args);