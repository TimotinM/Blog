using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogCoreEngine.Core.Entities
{
    public class Author : BaseEntity
    {
        private ICollection<PostDataModel> _posts;
        private ICollection<CommentDataModel> _comments;

        public Author() { }

        private ILazyLoader LazyLoader { get; set; }

        public Author(ILazyLoader lazyLoader)
        {
            this.LazyLoader = lazyLoader;
            this.Blogs = new HashSet<UsersBlogs>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] Image { get; set; }

          public ICollection<UsersBlogs> Blogs { get; set; }

          public ICollection<PostDataModel> Posts
        {
            get => this.LazyLoader.Load(this, ref _posts);
            set => _posts = value;
        }

        public ICollection<CommentDataModel> Comments
        {
            get => this.LazyLoader.Load(this, ref _comments);
            set => _comments = value;
        }
    }
}
