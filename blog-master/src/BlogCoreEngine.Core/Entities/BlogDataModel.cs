using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System;

namespace BlogCoreEngine.Core.Entities
{
     public class BlogDataModel : BaseEntity
     {
          private ICollection<PostDataModel> _posts;        

          public BlogDataModel() {
               this.Authors = new HashSet<UsersBlogs>();
          }

          private ILazyLoader LazyLoader { get; set; }

          public BlogDataModel(ILazyLoader lazyLoader)
          {
               this.LazyLoader = lazyLoader;
          }

          [Required]
          public string Name { get; set; }

          [Required]
          public string Description { get; set; }

          [Required]
          public byte[] Cover { get; set; }
          public  ICollection<UsersBlogs> Authors { get; set; }

          public ICollection<PostDataModel> Posts
          {
               get => this.LazyLoader.Load(this, ref _posts);
               set => _posts = value;
          }
          //public ICollection<IdentityUser> Users
          //{
          //     get => this.LazyLoader.Load(this, ref _users);
          //     set => _users = value;
          //}
     }
}
