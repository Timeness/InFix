namespace InfixBot.modules
{
    public static class Config
    {
        public static long OWNER => long.Parse(Environment.GetEnvironmentVariable("BOT_OWNER_ID") ?? "5896960462");
    }
}
