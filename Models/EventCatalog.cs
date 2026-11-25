namespace EventEase.Models;

/// <summary>Fixed lists used by forms and filters (categories and bundled images).</summary>
public static class EventCatalog
{
    public const string DefaultImage = "/images/default-event.svg";

    public static readonly string[] Categories =
    {
        "Conference", "Workshop", "Networking", "Entertainment", "Career", "Education", "Sports", "Other"
    };

    public static readonly (string Path, string Label)[] Images =
    {
        ("/images/default-event.svg", "Default"),
        ("/images/conference.svg", "Conference"),
        ("/images/workshop.svg", "Workshop"),
        ("/images/networking.svg", "Networking"),
        ("/images/entertainment.svg", "Entertainment"),
        ("/images/career.svg", "Career")
    };
}
