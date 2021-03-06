﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastersign.DashOps
{
    public interface IExecutable : ILogged
    {
        string Command { get; }

        string Arguments { get; }

        string WorkingDirectory { get; }

        int[] ExitCodes { get; }

        string CurrentLogFile { get; set; }

        string Title { get; }

        bool Visible { get; }

        bool KeepOpen { get; }

        bool AlwaysClose { get; }

        Func<string, bool> SuccessCheck { get; }

        void NotifyExecutionFinished();
    }
}
