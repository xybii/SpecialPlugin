using Quartz;
using System;

namespace SpecialPlugin.DapperOneDemo
{
    public interface IJobService : IJob, IDisposable
    {
    }
}
