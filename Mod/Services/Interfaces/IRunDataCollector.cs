using RoR2DevTool.Models;

namespace RoR2DevTool.Services.Interfaces
{
    public interface IRunDataCollector
    {
        bool IsInRun();
        RunData GetRunData();
    }
}
