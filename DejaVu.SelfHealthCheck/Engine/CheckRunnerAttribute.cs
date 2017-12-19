using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.Engine
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CheckRunnerAttribute : Attribute 
    {
        readonly Type _checkRunnerType;

        // This is a positional argument
        public CheckRunnerAttribute(Type checkRunnerType)
        {
            this._checkRunnerType = checkRunnerType;
        }

        public Type CheckRunnerType
        {
            get { return _checkRunnerType; }
        }

    }

}
