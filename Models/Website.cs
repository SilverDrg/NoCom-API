﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NoCom_API.Models
{
    /// <summary>
    /// Saving websites without keeping track what kind of websites the users are browsing on
    /// </summary>
    public partial class Website
    {
        public Website()
        {
            Comments = new HashSet<Comment>();
        }

        public long Id { get; set; }
        public string UrlHash { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
