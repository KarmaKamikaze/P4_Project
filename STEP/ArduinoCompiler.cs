﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ArduinoUploader;
using ArduinoUploader.Hardware;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace STEP;

public class ArduinoCompiler
{
    private readonly string _port;

    public ArduinoCompiler(string port)
    {
        _port = port;
    }

    /// <summary>
    /// The Upload method requires the arduino-cli executable to be present at the host machine.
    /// It first compiles the STEP compiler's output to binaries, then converts this output to Intel HEX format.
    /// The .hex file is then uploaded directly to the connected Arduino Uno board.
    /// </summary>
    public void Upload(string filename)
    {
        string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Check if arduino-cli files are present in correct folder
            if (!File.Exists($"{directoryPath}/ArduinoCLI/arduino-cli.exe"))
            {
                throw new ApplicationException("Please download the arduino-cli and place it in the ArduinoCLI folder.");
            }
            
            // The /C flag means that cmd should execute the following command and exit without waiting for further input.
            ExecuteProcess("cmd.exe", "/C " +
                                      $"{directoryPath}/ArduinoCLI/arduino-cli.exe " +
                                      "compile " +
                                      "--export-binaries " +
                                      "-b arduino:avr:uno " +
                                      $"{directoryPath}/{filename}/");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Check if arduino-cli files are present in correct folder
            if (!File.Exists($"{directoryPath}/ArduinoCLI/arduino-cli"))
            {
                throw new ApplicationException("Please download the arduino-cli and place it in the ArduinoCLI folder.");
            }
            
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli",
                "compile " +
                "--export-binaries " +
                "-b arduino:avr:uno " +
                $"{directoryPath}/{filename}/");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // Check if arduino-cli files are present in correct folder
            if (!File.Exists($"{directoryPath}/ArduinoCLI/arduino-cli"))
            {
                throw new ApplicationException("Please download the arduino-cli and place it in the ArduinoCLI folder.");
            }
            
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli",
                "compile " +
                "--export-binaries " +
                "-b arduino:avr:uno " +
                $"{directoryPath}/{filename}/");
        }
        else
        {
            throw new ApplicationException("OS is not supported by the C# Process initializer.");
        }

        if (!File.Exists(directoryPath + $"/{filename}/build/arduino.avr.uno/{filename}.ino.hex"))
        {
            throw new ApplicationException($"Arduino compiler error. {filename}.ino.hex file was not generated.");
        }

        // Enable upload logging
        var nlogConfig = new LoggingConfiguration();

        nlogConfig.AddRule(minLevel: LogLevel.Trace, maxLevel: LogLevel.Fatal,
            target: new ConsoleTarget("consoleTarget")
            {
                Layout = "${longdate} level=${level} message=${message}"
            });

        LogManager.Configuration = nlogConfig;

        // Update compiled hex file to Arduino board
        ArduinoSketchUploader uploader = new ArduinoSketchUploader(new ArduinoSketchUploaderOptions()
        {
            FileName = directoryPath + $"/{filename}/build/arduino.avr.uno/{filename}.ino.hex",
            PortName = _port, // Can be omitted to try to auto-detect the COM port.
            ArduinoModel = ArduinoModel.UnoR3
        }, new NLogArduinoUploaderLogger(error: true, warn: true, info: true));
        uploader.UploadSketch();
    }

    public void Monitor()
    {
        string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Console.WriteLine("\nEntering Output-mode...\n");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // The /C flag means that cmd should execute the following command and exit without waiting for further input.
            ExecuteProcess("cmd.exe", "/C " +
                                      $"{directoryPath}/ArduinoCLI/arduino-cli.exe " +
                                      "monitor " +
                                      $"-p {_port} " +
                                      "-b arduino:avr:uno");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli", 
                "monitor " +
                $"-p {_port} " +
                "-b arduino:avr:uno");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli", 
                "monitor " +
                $"-p {_port} " +
                "-b arduino:avr:uno");
        }
        else
        {
            throw new ApplicationException("OS is not supported by the C# Process initializer.");
        }
    }

    public void ListPorts()
    {
        string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // The /C flag means that cmd should execute the following command and exit without waiting for further input.
            ExecuteProcess("cmd.exe", $"/C {directoryPath}/ArduinoCLI/arduino-cli.exe board list");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli", $"board list");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ExecuteProcess($"{directoryPath}/ArduinoCLI/arduino-cli", $"board list");
        }
        else
        {
            throw new ApplicationException("OS is not supported by the C# Process initializer.");
        }
    }

    public void ExecuteProcess(string fileName, string arguments)
    {
        Process process = Process.Start(fileName, arguments);

        process?.WaitForExit();
    }

    private class NLogArduinoUploaderLogger : IArduinoUploaderLogger
    {
        private readonly bool _error;
        private readonly bool _warn;
        private readonly bool _info;
        private readonly bool _debug;
        private readonly bool _trace;

        public NLogArduinoUploaderLogger(bool error = false, bool warn = false, bool info = false, bool debug = false,
            bool trace = false)
        {
            _error = error;
            _warn = warn;
            _info = info;
            _debug = debug;
            _trace = trace;
        }

        private static readonly Logger Logger = LogManager.GetLogger("ArduinoSketchUploader");

        public void Error(string message, Exception exception)
        {
            if (_error) Logger.Error(exception, message);
        }

        public void Warn(string message)
        {
            if (_warn) Logger.Warn(message);
        }

        public void Info(string message)
        {
            if (_info) Logger.Info(message);
        }

        public void Debug(string message)
        {
            if (_debug) Logger.Debug(message);
        }

        public void Trace(string message)
        {
            if (_trace) Logger.Trace(message);
        }
    }
}