namespace Demo.Tact.Console.Configuration
{
    [RegisterConfiguration]
    public class DemoConfig
    {
        [IsValidationEnabled]
        public bool IsEnable { get; set; }

        [RequireNonDefault]
        public string SomeString { get; set; }

        [RequireNonDefault]
        public int Thing1 { get; set; }

        [RequireNonDefault]
        public bool? Thing2 { get; set; }

        [RequireNonDefault]
        public bool? Thing3 { get; set; }
    }
}