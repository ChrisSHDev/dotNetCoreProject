using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatSAP.Helpers;
using WhatSAP.Models;
using WhatSAP.Models.Account;

namespace WhatSAP.Controllers
{
    public class AccountsController : Controller
    {
        private readonly WhatSAPContext _context;
        public AccountsController(WhatSAPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //GET   /Accounts/login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                if (HttpContext.Session.GetString("userType").Equals("Customer"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (HttpContext.Session.GetString("userType").Equals("Client"))
                {
                    return RedirectToAction("Index", "Client");
                }
                else if (HttpContext.Session.GetString("userType").Equals("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View(new LoginViewModel());
        }

        //POST /Accounts/login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email, Password, UserType")]LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                if (loginViewModel != null)
                {
                    switch (loginViewModel.UserType)
                    {
                        case "Customer":
                            Customer customer = FindCustomerByEmail(loginViewModel.Email);
                            if (customer == null)
                            {
                                ModelState.AddModelError("LoginError", "Email address/Password does not exist");
                                return View();
                            }
                            if (customer.Password.Equals(EncryptionPassword(loginViewModel.Password)))
                            {
                                HttpContext.Session.SetInt32("token", (int)customer.CustomerId);
                                HttpContext.Session.SetString("user", customer.FirstName + " " + customer.LastName);
                                HttpContext.Session.SetString("userType", loginViewModel.UserType);
                                SessionHelper.SetObjectAsJson(HttpContext.Session, "customer", customer);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                //TODO: exception password and email doesnt match
                                ModelState.AddModelError("LoginError", "Email address/Password does not exist");
                                return View();
                            }
                        case "Client":
                            Client client = FindClientByEmail(loginViewModel.Email);
                            if(client == null)
                            {
                                ModelState.AddModelError("LoginError", "Email address/Password does not exist");
                                return View();
                            }
                            if (client.Password.Equals(EncryptionPassword(loginViewModel.Password)))
                            {
                                HttpContext.Session.SetInt32("token", (int)client.ClientId);
                                HttpContext.Session.SetString("user", client.FirstName + " " + client.LastName);
                                HttpContext.Session.SetString("userType", loginViewModel.UserType);
                                SessionHelper.SetObjectAsJson(HttpContext.Session, "client", client);
                                return RedirectToAction("Index", "Client", new {id = client.ClientId });
                            }
                            else
                            {
                                //TODO: exception password and email doesnt match
                                ModelState.AddModelError("LoginError", "Email address/Password does not exist");
                                return View();
                            }
                        default:
                            ViewData["Message"] = "Choose your user type";
                            break;
                    }
                }
                else
                {
                    ViewData["Message"] = "Email address or Password doesn't exist";
                }
            }
            return View();
        }
        
        //GET /Accounts/index
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        //GET /Accounts/register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName,LastName,EmailAddress,Password,ConfirmPassword,UserType")]RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if(!UserExists(registerViewModel.EmailAddress))
                {
                    switch (registerViewModel.UserType)
                    {
                        case "Customer":
                            Customer customer = new Customer
                            {
                                Email = registerViewModel.EmailAddress,
                                Password = EncryptionPassword(registerViewModel.Password),
                                FirstName = registerViewModel.FirstName,
                                LastName = registerViewModel.LastName
                            };
                            _context.Customer.Add(customer);
                            break;
                        case "Client":
                            Client client = new Client
                            {
                                Email = registerViewModel.EmailAddress,
                                Password = EncryptionPassword(registerViewModel.Password),
                                FirstName = registerViewModel.FirstName,
                                LastName = registerViewModel.LastName
                            };
                            _context.Client.Add(client);
                            break;
                        default:
                            throw new ArgumentNullException();
                    }
                    await _context.SaveChangesAsync();
                    ViewData["Message"] = "Registration succeed";
                    return View("Login");
                }
            }

            //TODO: Need to create UI for exception w/ message choose different email address
            throw new Exception("user email has already exist");
        }
        private bool UserExists(string email) 
        {
            bool cuEmailValidation = _context.Customer.Any(e => e.Email.Equals(email));
            bool clEmailValidation = _context.Client.Any(e => e.Email.Equals(email));

            if(cuEmailValidation || clEmailValidation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Customer FindCustomerByEmail(string email)
        {
            try
            {
                Customer user = _context.Customer.SingleOrDefault(customer => customer.Email.Equals(email));
                return _context.Customer.SingleOrDefault(customer => customer.Email.Equals(email));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private Client FindClientByEmail(string email)
        {
            return _context.Client.SingleOrDefault(client => client.Email.Equals(email));
        }
        public static string EncryptionPassword(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            UTF8Encoding encode = new UTF8Encoding();

            byte[] en = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < en.Length; i++)
            {
                sb.Append(en[i].ToString());
            }

            return sb.ToString();
        }
    }
}