namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeRouting();
            InitializeComponent();
        }
        private void InitializeRouting()
        {
            var navigationItems = new List<NavigationItem>
            {
                new NavigationItem
                {
                    Title = "Main",
                    PageType = typeof(MainPage),
                    Route = "MainPage"
                }
            };

            foreach (var item in navigationItems.Select(x => new FlyoutItem
            {
                IsVisible = x.IsVisible,
                FlyoutItemIsVisible = x.IsVisible,
                Title = x.Title,
                Route = x.Route,
                Items =
            {
                new Tab
                {
                    Title = x.Title,
                    Items =
                    {
                        new ShellContent
                        {
                            Title = x.Title,
                            Route = x.Route,
                            ContentTemplate = new DataTemplate(x.PageType)
                        }
                    }
                }
            }
            }))
                Items.Add(item);
        }
    }

    public class NavigationItem
    {
        public string? Title { get; set; }
        public Type? PageType { get; set; }
        public ImageSource? Image { get; set; }
        public string? Route { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}