using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Core.Models.Events
{
    public class ShortcutAddedEvent : IEvent
    {
        public ShortcutMetaData ShortcutMetaData { get; }

        public ShortcutAddedEvent(ShortcutMetaData shortcutMetadata)
        {
            ShortcutMetaData = shortcutMetadata;
        }
    }
}
