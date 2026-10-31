using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NewaccNet.AI.Skills
{
    public class SkillDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string DraftType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tools { get; set; } = new();
        public int MaxToolRounds { get; set; } = 8;
        public string SystemPrompt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Nạp và phân giải các tệp Skill định dạng Markdown (.md) có chứa YAML Frontmatter.
    /// Giúp tách rời hướng dẫn nghiệp vụ của AI ra khỏi code C#.
    /// </summary>
    public static class SkillLoader
    {
        public static SkillDefinition? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var content = File.ReadAllText(filePath);
            return ParseMarkdown(content);
        }

        public static SkillDefinition ParseMarkdown(string markdownContent)
        {
            var skill = new SkillDefinition();
            if (string.IsNullOrWhiteSpace(markdownContent)) return skill;

            var lines = markdownContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            if (lines.Length > 0 && lines[0].Trim() == "---")
            {
                int endFrontmatterIndex = -1;
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].Trim() == "---")
                    {
                        endFrontmatterIndex = i;
                        break;
                    }
                }

                if (endFrontmatterIndex != -1)
                {
                    // Parse YAML frontmatter đơn giản
                    for (int i = 1; i < endFrontmatterIndex; i++)
                    {
                        var line = lines[i].Trim();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                        var colonIdx = line.IndexOf(':');
                        if (colonIdx > 0)
                        {
                            var key = line.Substring(0, colonIdx).Trim().ToLowerInvariant();
                            var val = line.Substring(colonIdx + 1).Trim();

                            switch (key)
                            {
                                case "name":
                                    skill.Name = val;
                                    break;
                                case "drafttype":
                                    skill.DraftType = val;
                                    break;
                                case "description":
                                    skill.Description = val;
                                    break;
                                case "maxtoolrounds":
                                    if (int.TryParse(val, out var rounds)) skill.MaxToolRounds = rounds;
                                    break;
                                case "tools":
                                    // Dạng [a, b, c]
                                    var cleanedVal = val.Trim('[', ']');
                                    skill.Tools = cleanedVal.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                            .Select(t => t.Trim())
                                                            .ToList();
                                    break;
                            }
                        }
                    }

                    // Toàn bộ phần còn lại sau `---` là PromptBody
                    skill.SystemPrompt = string.Join(Environment.NewLine, lines.Skip(endFrontmatterIndex + 1)).Trim();
                }
                else
                {
                    skill.SystemPrompt = markdownContent;
                }
            }
            else
            {
                skill.SystemPrompt = markdownContent;
            }

            return skill;
        }

        public static SkillDefinition? FindSkillForDraftType(string draftType, string? skillsDirectory = null)
        {
            var dir = skillsDirectory;
            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills");
                if (!Directory.Exists(dir))
                {
                    var sourceSkills = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "NewaccNet.AI", "Skills");
                    if (Directory.Exists(sourceSkills)) dir = Path.GetFullPath(sourceSkills);
                }
            }

            if (!Directory.Exists(dir)) return null;

            var files = Directory.GetFiles(dir, "*.md");
            foreach (var f in files)
            {
                var skill = LoadFromFile(f);
                if (skill != null && string.Equals(skill.DraftType, draftType, StringComparison.OrdinalIgnoreCase))
                {
                    return skill;
                }
            }

            return null;
        }
    }
}
