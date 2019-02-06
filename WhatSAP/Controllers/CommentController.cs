using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatSAP.Models;

namespace WhatSAP.Controllers
{
    public class CommentController : Controller
    {
        private readonly WhatSAPContext _context;
        public CommentController(WhatSAPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId, Body, Rate, CustomerId, ActivityId")]Comment comment)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("token") != null)
                {
                    long userId = (long)HttpContext.Session.GetInt32("token");
                    comment.CustomerId = userId;
                    comment.Customer = _context.Customer.Where(x => x.CustomerId.Equals(userId)).FirstOrDefault();
                    comment.Activity = _context.Activity.Where(x => x.ActivityId.Equals(comment.ActivityId)).FirstOrDefault();
                }
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Activity", new { id = comment.ActivityId });
            }
            return View(comment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var comment = await _context.Comment.FindAsync(id);
            var activityId = comment.ActivityId;
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Activity", new { id = activityId });
        }
    }
}