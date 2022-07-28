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
    public interface ICommand
    {
        string? Call(string[] args);
    }
}