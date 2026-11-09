using System.Collections.Generic;
using NewaccNet.Contracts.AI;

namespace NewaccNet.Contracts.Plugins
{
    /// <summary>
    /// Plugin có dùng AI thì implement.
    /// </summary>
    public interface IPluginAiContributor
    {
        IEnumerable<SkillSource> GetSkills();       // nội dung .md nhúng
        IEnumerable<IAiTool> GetTools();
    }

    public sealed record SkillSource(string FileName, string Content);
}
