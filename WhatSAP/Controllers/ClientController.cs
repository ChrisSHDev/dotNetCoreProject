using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatSAP.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Mail;
using System.Net;

namespace WhatSAP.Controllers
{
    public class ClientController : Controller
    {

        private readonly WhatSAPContext _context;

        public ClientController(WhatSAPContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _context.Client.Include(c => c.Activity).Include(c => c.Booking)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        public async Task<IActionResult> ActivityRequest(long? id)
        {
            ViewBag.ClientId = id;

            var activity = await _context.Activity
                .Include(c => c.Address)
                .Include(c => c.Category)
                .Include(c => c.Client)
                .Where(m => m.ClientId == id)
                .Where(m => m.Authorized.Equals(false))
                .ToArrayAsync();

            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        public async Task<IActionResult> ActivityList(long? id)
        {
            var activity = await _context.Activity
                .Include(c => c.Address)
                .Include(c => c.Category)
                .Include(c => c.Client)
                .Where(m => m.ClientId == id)
                .ToArrayAsync();

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        public IActionResult RequestForm()
        {
            Activity activity = new Activity();
            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestForm([Bind("ActivityId", "ActivityName", "Description", "ActivityDate", "Price", "ClientId")]Activity activity, IFormFile file)
        {
            if (file == null)
                return Content("File is not selected");
            Path.GetTempFileName();
            if (ModelState.IsValid)
            {
                activity.ClientId = (long)HttpContext.Session.GetInt32("token");
                _context.Activity.Add(activity);
            }
            await _context.SaveChangesAsync();

            CloudBlobContainer container = BlobsController.GetClouldBlobContainer();
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(activity.ActivityId + "_" + file.FileName);
            var stream = file.OpenReadStream();

            await cloudBlockBlob.UploadFromStreamAsync(stream);
            stream.Dispose();

            activity.RequestFormPath = "https://whatsapstorage.blob.core.windows.net/whatsap/" + activity.ActivityId + "_" + file.FileName;
            _context.Update(activity);
            await _context.SaveChangesAsync();


            return RedirectToAction("ActivityRequest", "Client", new { id = activity.ClientId });
        }

        public async Task<IActionResult> Download()
        {
            var filename = "ActivityReqeustForm.docx";
            CloudBlobContainer container = BlobsController.GetClouldBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(filename);

            Stream blobStream = await blob.OpenReadAsync();

            return File(blobStream, blob.Properties.ContentType, filename);

        }

        public IActionResult BookingList(long? id)
        {
            if (id == null)
                return NotFound();

            var group =
                from b in _context.Booking
                join c in _context.Customer on b.CustomerId equals c.CustomerId
                where b.ClientId == id
                select new { bookingId = b.BookingId, activity = b.Activity, customer = c, num = b.NumberOfPeople, total = b.Total, bookingDate = b.BookingDate, confirmed = b.Confirmed };

            List<BookingViewModel> bookings = new List<BookingViewModel>();

            foreach (var g in group)
            {
                BookingViewModel b = new BookingViewModel(g.bookingId, g.activity, g.customer, g.num, g.total, g.bookingDate, g.confirmed);
                bookings.Add(b);
            }

            return View(bookings);
        }

        public async Task<IActionResult> Confirm(long id)
        {
            var booking = _context.Booking.FirstOrDefault(x => x.BookingId == id);
            booking.Confirmed = true;
            await _context.SaveChangesAsync();

            SmtpClient client = new SmtpClient("mysmtpserver");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("username", "password"); //TODO: Change 

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("WhatsapAdmin@whatsap.com");
            mailMessage.To.Add(booking.Customer.Email);
            mailMessage.Body = "Your booking for " + booking.Activity.ActivityName + " has been approved. Please pay for the activity";
            mailMessage.Subject = "Booking Confirmation";
            client.Send(mailMessage);

            return RedirectToAction("BookingList", new { id = booking.ClientId });
        }

        public async Task<IActionResult> Reject(long id)
        {
            var booking = _context.Booking.FirstOrDefault(x => x.BookingId == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            SmtpClient client = new SmtpClient("mysmtpserver");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("username", "password"); //TODO: Change 

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("WhatsapAdmin@whatsap.com");
            mailMessage.To.Add(booking.Customer.Email);
            mailMessage.Body = "Your booking for " + booking.Activity.ActivityName + " has been rejected.";
            mailMessage.Subject = "Booking Rejection";
            client.Send(mailMessage);

            return RedirectToAction("BookingList", new { id = booking.ClientId });
        }
    }
}