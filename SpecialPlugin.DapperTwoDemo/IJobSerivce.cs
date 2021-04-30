using Quartz;
using System;

namespace SpecialPlugin.DapperTwoDemo
{
    public interface IJobService : IJob, IDisposable
    {
    }
}
