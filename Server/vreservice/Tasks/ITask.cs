using System;
using Vre.Server.Command;

namespace Vre.Server.Task
{
    internal interface ITask
    {
        string Name { get; }
        void Execute(Parameters param);
    }
}