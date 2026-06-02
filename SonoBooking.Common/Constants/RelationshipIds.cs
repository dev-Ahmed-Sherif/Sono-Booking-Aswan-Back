namespace SonoBooking.Common.Constants
{
    /// <summary>
    /// Seeded relationship lookup IDs. Father and mother may appear only once per user.
    /// </summary>
    public static class RelationshipIds
    {
        public const string Father = "019f0d6c-3a9b-7a5c-9c9b-5b44b1a1e001";
        public const string Mother = "019f0d6c-3a9b-7a5c-9c9b-5b44b1a1e002";

        public static bool IsUniquePerUser(string relationshipId) =>
            relationshipId == Father || relationshipId == Mother;
    }
}
