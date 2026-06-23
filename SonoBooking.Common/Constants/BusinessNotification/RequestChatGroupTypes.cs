using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Common.Constants.BusinessNotification
{
    public static class RequestChatGroupTypes
    {
        public const string OwnerLeader = "owner-leader";
        public const string LeaderReception = "leader-reception";
        public const string OwnerReception = "owner-reception";

        public static IReadOnlyList<string> All { get; } =
        [
            OwnerLeader,
            LeaderReception,
            OwnerReception
        ];

        public static bool IsValid(string? groupType) =>
            !string.IsNullOrWhiteSpace(groupType) &&
            All.Any(value => string.Equals(value, groupType.Trim(), StringComparison.Ordinal));

        public static (string NameAr, string NameEn) GetDisplayNames(string groupType) =>
            groupType switch
            {
                OwnerLeader => ("صاحب الطلب ↔ القائد", "Request owner ↔ Leader"),
                LeaderReception => ("القائد ↔ موظف الحجز", "Leader ↔ Reservation employee"),
                OwnerReception => ("صاحب الطلب ↔ موظف الحجز", "Request owner ↔ Reservation employee"),
                _ => ("محادثة", "Conversation")
            };
    }
}
