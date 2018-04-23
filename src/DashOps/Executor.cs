﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastersign.DashOps
{
    public class Executor
    {
        public bool IsValid(ActionView action) => true;

        public void Execute(ActionView action, string logDirectory, string logName)
        {
            bool CanLog() => logDirectory != null;
            string LogPath() => System.IO.Path.Combine(logDirectory, logName);

            var expandedCommand = Environment.ExpandEnvironmentVariables(action.Command);
            var expandedArgs = CommandLine.FormatArgumentList(action.Arguments.Select(Environment.ExpandEnvironmentVariables).ToArray());
            var psLines = new List<string>();
            if (CanLog()) psLines.Add($"$_ = Start-Transcript -Path \"{LogPath()}\"");
            psLines.Add($"echo \"Command:   {expandedCommand}\"");
            if (!string.IsNullOrWhiteSpace(expandedArgs)) psLines.Add($"echo \"Arguments: {expandedArgs.Replace("\"", "`\"")}\"");
            psLines.Add($"& \"{expandedCommand}\" {expandedArgs}");
            psLines.Add("$success = $?");
            psLines.Add("$exitCode = $LastExitCode");
            if (CanLog()) psLines.Add("$_ = Stop-Transcript");
            psLines.Add("echo \"\"");
            psLines.Add("if (!$success) { if ($LastExitCode -ne $null) { Write-Warning \"Exit Code: $exitCode\" } else { Write-Warning \"Command failed.\" } }");
            psLines.Add("echo \"Press any key to continue...\"");
            psLines.Add("$_ = $Host.UI.RawUI.ReadKey()");
            var cmd = string.Join("; ", psLines);
            var encodedCmd = EncodePowerShellCommand(cmd);
            var psArgs = $"-NoLogo -ExecutionPolicy RemoteSigned -EncodedCommand {encodedCmd}";
            var psi = new ProcessStartInfo(CommandLine.PowerShellExe, psArgs)
            {
                WindowStyle = ProcessWindowStyle.Normal
            };
            Process.Start(psi);
        }

        private string EncodePowerShellCommand(string cmd)
            => Convert.ToBase64String(Encoding.Unicode.GetBytes(cmd));
    }
}
