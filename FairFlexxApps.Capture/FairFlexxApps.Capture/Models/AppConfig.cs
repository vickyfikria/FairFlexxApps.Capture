using FairFlexxApps.Capture.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Models
{
    [Table("AppConfig")]
    public class AppConfig
    {
        [PrimaryKey]
        public int Id { get; set; } = 0;

        public MagicForm MagicForm { get; set; } = 0;
        public MagicCard MagicCard { get; set; } = 0;

        public bool Visitor_Multi { get; set; } = true;
        public bool Visitor_Auto { get; set; } = true;
        public bool Visitor_Flash { get; set; }

        public bool Attach_Multi { get; set; }
        public bool Attach_Auto { get; set; } = true;
        public bool Attach_Flash { get; set; }

        public bool Object_Multi { get; set; }
        public bool Object_Flash { get; set; }
    }
}
