using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EadWebProject.Models
{
    public class ProfileWithLikes
    {
        public Profile MyProfile { get; set; }

        public int NoOfLikes { get; set; }

        public int NoOfDislikes { get; set; }
    }
}