using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreEngine.Core.Entities
{
     public class UsersBlogs
     {
          public Guid? UserId { get; set; }
          public Guid? BlogId { get; set; }
          public Author User { get; set; }
          public BlogDataModel Blog { get; set; }

     }
}
