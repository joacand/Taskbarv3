using System.Collections.Generic;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Interfaces
{
    public interface IShortcutsHandler : IFileHandler<IEnumerable<Shortcut>>
    { }
}
