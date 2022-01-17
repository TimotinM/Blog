using BlogCoreEngine.Core.Entities;
using BlogCoreEngine.Core.Interfaces;
using BlogCoreEngine.Web.Extensions;
using BlogCoreEngine.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BlogCoreEngine.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BlogCoreEngine.Controllers
{
    public class BlogController : Controller
    {
        private readonly IAsyncRepository<BlogDataModel> blogRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

          public BlogController(IAsyncRepository<BlogDataModel> blogRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
          {
            this.blogRepository = blogRepository;
            this.userManager = userManager;
            this.context = context;
          }

        #region View

          [Authorize]
        public async Task<IActionResult> View(Guid id)
        {
            var blog = await context.Blogs.Include(x => x.Authors).SingleOrDefaultAsync(x => x.Id == id);
            var curUser = await userManager.GetUserAsync(HttpContext.User);
            var users = blog.Authors.Select(x => x.UserId).ToList();
               if (users.Contains(curUser.AuthorId))
               {
                    return View(blog);
               }
               var userBlog = new UsersBlogs { UserId = curUser.AuthorId, BlogId = id };
               return PartialView("_BuyBlog", userBlog);  
          }

        #endregion

        #region New

        [Authorize(Roles = "Administrator")]
        public IActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Administrator"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> New(BlogViewModel blog)
        {
            if (ModelState.IsValid)
            {
                var newBlog = await this.blogRepository.Add(new BlogDataModel
                {
                    Id = Guid.NewGuid(),
                    Name = blog.Name,
                    Description = blog.Description,
                    Cover = blog.Cover.ToByteArray(),
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                });

                return this.RedirectToAsync<BlogController>(x => x.View(newBlog.Id));
            }
            return View(blog);
        }

        #endregion

        #region Edit

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(Guid id)
        {
            return View(await this.blogRepository.GetById(id));
        }

        [Authorize(Roles = "Administrator"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogDataModel blog, IFormFile formFile)
        {
            var targetBlog = await this.blogRepository.GetById(id);

            if (ModelState.IsValid)
            {
                targetBlog.Name = blog.Name;
                targetBlog.Description = blog.Description;

                if (!(formFile == null || formFile.Length <= 0))
                {
                    targetBlog.Cover = formFile.ToByteArray();
                }

                await this.blogRepository.Update(targetBlog);

                return this.RedirectToAsync<BlogController>(x => x.View(id));
            }

            blog.Cover = targetBlog.Cover;

            return View(blog);
        }

        #endregion

        #region Delete

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await this.blogRepository.Remove(id);
            return this.RedirectToAsync<HomeController>(x => x.Index());
        }

        #endregion
          
       [Authorize]
       public async Task<IActionResult> Buy(Guid blogId, Guid userId)
       {
               var user = new UsersBlogs { BlogId = blogId, UserId = userId };
               await context.UsersBlogs.AddAsync(user);
               await context.SaveChangesAsync();
               return this.RedirectToAsync<BlogController>(x => x.View(blogId));
       }
     
    }
}