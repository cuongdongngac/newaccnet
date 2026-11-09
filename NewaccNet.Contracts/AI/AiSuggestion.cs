using System;
using System.Collections.Generic;

namespace NewaccNet.Contracts.AI
{
    public enum SuggestionOutcome
    {
        Pending,
        Accepted,
        Edited,
        Rejected
    }

    public sealed class AiSuggestion
    {
        public string SuggestionId { get; set; } = Guid.NewGuid().ToString("N");
        public int LineNo { get; set; }                  // 0 = cấp chứng từ / đối tác
        public string FieldPath { get; set; } = "";      // "Counterparty.PartnerId", "Lines[3].MaterialId"
        public string Value { get; set; } = "";
        public double Confidence { get; set; }           // 0.0–1.0, MỘT thang duy nhất toàn hệ thống
        public string Reason { get; set; } = "";
        public List<string> Evidence { get; set; } = new();  // vd "search_materials: khớp 'SUA TUOI TH 1L'"
        public MatchMethod Method { get; set; } = MatchMethod.Ai;
        public string SkillId { get; set; } = "";
        public string SkillVersion { get; set; } = "";
        public string ModelUsed { get; set; } = "";
        public string CallId { get; set; } = "";         // → AiCallLog
        public SuggestionOutcome Outcome { get; set; } = SuggestionOutcome.Pending;
        public string? FinalValue { get; set; }          // giá trị người dùng chốt
    }
}
