namespace IntCopilot.Barker.Worker.Data
{
    public class Profile
    {
        private readonly ChangeNotifier _onChanged;

        public Profile()
        {
            _onChanged = new ChangeNotifier(this);
            Id = Guid.NewGuid().ToString();
        }

        public required string Description { get; set; }
        public string Id { get; }
        public required string StudentId { get; set; }
        public required string SchoolYearId { get; set; }
        public Func<TimeWindow> TimeWindowProvider { get; set; } = () => TimeWindow.Today;
        public TimeWindow TimeWindow => TimeWindowProvider();
        public ChangeNotifier OnChanged => _onChanged;
        public List<Subscription> Subscriptions { get; } = new();
    }
}