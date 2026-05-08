namespace SonoBooking.Common.Constants.EmailConstants
{
    public static class EmailBody
    {
        public static string GetRegistrationConfirmationBody(string password)
        {
            return @$"
                تم إضافة حسابك كمستخدم للجمعية ،
                وكلمة السر هي {password}
                ويمكنك تغييرها لاحقاً.
                ";
        }
    }
}

