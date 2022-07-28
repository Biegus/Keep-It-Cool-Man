#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace Cyberultimate.Unity
{
    public class CallComendArgs : EventArgs
    {

        public CyberCommand? Command { get; }
        public string FullCode { get; }
        public string[] Args { get; }
        public string? ErrorCode { get; }
        public bool WasCorrect => ErrorCode == null;
        public Exception? Exception { get; }

        public CallComendArgs(CyberCommand? command, string fullCode, string[] args, Exception? exception, string? errorCode)
        {
            Command = command;
            FullCode = fullCode;
            Args = args;

            Exception = exception;
            ErrorCode = errorCode;
        }

    }
}