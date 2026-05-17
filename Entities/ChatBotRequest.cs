using System.Collections.Generic;

namespace Entities
{
    public record ChatBotRequest(
        string Message,
        List<HistoryItem> History,
        List<object> Products);
}

