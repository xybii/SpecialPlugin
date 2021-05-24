using Quartz;
using System;

namespace SpecialPlugin.Project.NewDapperDemo
{
    public interface IJobService : IJob, IDisposable
    {
    }
}
