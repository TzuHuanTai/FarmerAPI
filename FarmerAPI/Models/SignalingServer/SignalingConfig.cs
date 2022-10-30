namespace FarmerAPI.Models.SignalingServer
{
    public class SignalingConfig
    {
        public string Url { get; set; }

        public int RecheckingTime { get; set; }
    }

    public static class SignalingMethod
    {
        public static string OfferSDP => "OfferSDP";

        public static string AnswerSDP => "AnswerSDP";

        public static string OfferICE => "OfferICE";

        public static string AnswerICE => "AnswerICE";

        public static string ConnectedClient => "ConnectedClient";
    }

    public static class SignalingGroup
    {
        public static string Server => "Server";

        public static string Client => "Client";
    }
}