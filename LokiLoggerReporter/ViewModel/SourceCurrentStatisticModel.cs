using System.Collections.Generic;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.ViewModel
{
    public class SourceCurrentStatisticModel
    {
        public Source Source;
        public int Count;
        public int AllCount;
        public List<KeyValuePair<LogLevel, int>> Level;
        public List<KeyValuePair<LogTyp, int>> Typ;
    }
}