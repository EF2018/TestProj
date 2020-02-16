using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestProj.Models;
using TestProj.Models.Entities;
using TestProj.Models.ViewModels;

namespace TestProj.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message, int? from, int? to)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Настроен поставщик двухфакторной проверки подлинности."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : message == ManageMessageId.AddPhoneSuccess ? "Ваш номер телефона добавлен."
                : message == ManageMessageId.RemovePhoneSuccess ? "Ваш номер телефона удален."
                : message == ManageMessageId.AddNewPeriod ? "Новый плановый период добавлен."
                : message == ManageMessageId.UpdatePlanRecord ? "Плановый период обновлен."
                : message == ManageMessageId.AddNewFactRecord ? "Новая факт.запись добавлена."
                : message == ManageMessageId.UpdateFactRecord ? "Факт.запись обновлена."
                : message == ManageMessageId.UsersAccessToReportsUpdated? "Доступы пользователей к отчетам обновлены."
                : "";

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                Cities = new SelectList(GetAllCities(), "Id", "Name"),
            };

            var ff = UserManager.IsInRole(userId, "admin");

            if (user.Report1 || UserManager.IsInRole(userId, "admin"))
            {
                model.IsReport1 = true;
                model.Report1 = GetReport1(from, to);
            }
            if (user.Report2|| UserManager.IsInRole(userId, "admin"))
            {
                model.IsReport2 = true;
                model.Report2 = GetReport2();
            }
            if (UserManager.IsInRole(userId, "admin"))
            {
                model.IsAdmin = true;
                model.Users = GetUsers();
            }
            return View(model);
        }

        //
        //  GET: /Manage/Edit users roles policy
        [HttpGet]
        public ActionResult UsersList()
        {
            var list = GetUsers();
            return View(list);
        }


        // POST: /Manage/Edit users roles policy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UsersList(List<UserForListViewModel> modelList)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            
            foreach (var model in modelList)
            {
                //context.Users.Where(x=>x.)
                var user = UserManager.FindById(model.Id.ToString());
                user.Report1 = model.Report1;
                user.Report2 = model.Report2;
                UserManager.Update(user);
                context.SaveChanges();
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.UsersAccessToReportsUpdated });
        }


        //
        //  GET: /Manage/Create new plan or edit plan
        public ActionResult CreateNewPlan(int id = 0)
        {
            if (id != 0)
            {
                var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

                var planRecord = (from c in context.PlanRecords
                                  .Include("From")
                                  .Include("To")
                                  where c.Id == id
                                  select c).FirstOrDefault();
                CreateNewPlanViewModel model = new CreateNewPlanViewModel()
                {
                    Id = planRecord.Id,
                    From = planRecord.From.Name,
                    To = planRecord.To.Name,
                    Period = planRecord.Month,
                    Plan = planRecord.Number
                };
                ViewBag.Title = "Edit plan";
                return View("CreateNewPlan", model);
            }
            ViewBag.Title = "Create new plan for delivery";
            return View();
        }


        // POST: /Manage/Create new plan or edit plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewPlan(CreateNewPlanViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    PlanRecord planRecord = new PlanRecord()
                    {
                        From = GetCity(model.From),
                        To = GetCity(model.To),
                        Month = model.Period,
                        Number = model.Plan
                    };
                    context.PlanRecords.Add(planRecord);
                    context.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddNewPeriod });
                }
                var planRecordForUpdate = (from c in context.PlanRecords
                                           .Include("From")
                                           .Include("To")
                                           where c.Id == model.Id
                                           select c).FirstOrDefault();
                planRecordForUpdate.From = GetCity(model.From);
                planRecordForUpdate.To = GetCity(model.To);
                planRecordForUpdate.Month = model.Period;
                planRecordForUpdate.Number = model.Plan;
                context.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.UpdatePlanRecord });
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        //  GET: /Manage/Create new fact or edit fact
        public ActionResult CreateNewFact(int? idPlan, int id = 0)
        {
            if (id != 0)
            {
                var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

                var factRecord = (from c in context.FactRecords.Include("PlanRecord")
                                  where c.Id == id
                                  select c).FirstOrDefault();
                CreateNewFactRecordViewModel model = new CreateNewFactRecordViewModel()
                {
                    Id = factRecord.Id,
                    Date = factRecord.Date,
                    Number = factRecord.Number,
                    IdPlanRecord = factRecord.PlanRecord.Id
                };
                ViewBag.Title = "Edit fact record";
                return View("CreateNewFact", model);
            }
            CreateNewFactRecordViewModel modelVM = new CreateNewFactRecordViewModel()
            {
                IdPlanRecord = (int)idPlan
            };
            ViewBag.Title = "Create new fact record";
            return View(modelVM);
        }

        // POST: /Manage/Create new fact record or edit fact record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewFact(CreateNewFactRecordViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    FactRecord factRecord = new FactRecord()
                    {
                        Date = model.Date,
                        Number = model.Number,
                        PlanRecord = GetPlanRecord(model.IdPlanRecord)
                    };
                    context.FactRecords.Add(factRecord);
                    context.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddNewFactRecord });
                }
                var factRecordForUpdate = (from c in context.FactRecords
                                           where c.Id == model.Id
                                           select c).FirstOrDefault();
                factRecordForUpdate.Date = model.Date;
                factRecordForUpdate.Number = model.Number;
                context.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.UpdateFactRecord });
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Создание и отправка маркера
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Ваш код безопасности: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Отправка SMS через поставщик SMS для проверки номера телефона
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // Это сообщение означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Не удалось проверить телефон");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения

        private List<FactRecord> GetDayFactRecords(int idPlanRecord)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var outlist = context.FactRecords.Where(x => x.PlanRecord.Id == idPlanRecord).ToList();
            return outlist;
        }

        private IEnumerable GetAllCities()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var cities = context.Cities.ToList();
            cities.Insert(0, new City { Name = "Все", Id = 0 });
            return cities;
        }

        private City GetCity(string city)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var cityForSearch = context.Cities.FirstOrDefault(x => x.Name == city);
            if (cityForSearch == null)
            {
                City cityObj = new City() { Name = city };
                context.Cities.Add(cityObj);
                context.SaveChanges();
                cityForSearch = context.Cities.FirstOrDefault(x => x.Name == city);
            }
            return cityForSearch;
        }

        private PlanRecord GetPlanRecord(int idPlanRecord)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            return context.PlanRecords.FirstOrDefault(x => x.Id == idPlanRecord);
        }

        private List<Report2ViewModel> GetReport2()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var grouped = context.PlanRecords.GroupBy(d => new { d.From, d.To })
                .Select(group => new ConsolidatePlan() { FromTo = new Delivery() { From = group.Key.From, To = group.Key.To }, FactNumber = group.Sum(fact => fact.DayFactRecords.Sum(x => x.Number)), PlanNumber = group.Sum(plan => plan.Number) }).ToList();
            var newgrouped = grouped.GroupBy(d => new { d.FromTo.From }).ToList()
                .Select(group => new Report2ViewModel() { From = group.Key.From.Name, To = group.ToList() }).ToList();
            return newgrouped;
        }

        
        private List<Report1ViewModel> GetReport1(int? from, int? to)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            List<Report1ViewModel> reports = new List<Report1ViewModel>();
            var list = context.PlanRecords.Include("From")
                         .Include("To")
                         .Include("DayFactRecords").ToList();
            if (from!=0&& from!=null)
            {
                list = list.Where(x => x.From.Id == from).ToList();
            }
            if (to!=0&&to!=null)
            {
                list = list.Where(x => x.To.Id == to).ToList();
            }
            
            foreach (var item in list)
            {
                Report1ViewModel report1 = new Report1ViewModel()
                {
                    Id = item.Id,
                    From = item.From.Name,
                    To = item.To.Name,
                    Period = item.Month,
                    Plan = item.Number,
                    DayFactRecords = GetDayFactRecords(item.Id)
                };
                reports.Add(report1);
            }
            return reports;
        }

        public List<UserForListViewModel> GetUsers()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var curUserId = User.Identity.GetUserId();
            var users = context.Users.Where(x=>x.Id!= curUserId).ToList();

            List<UserForListViewModel> list = new List<UserForListViewModel>();
            foreach (var item in users)
            {
                UserForListViewModel user = new UserForListViewModel()
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Report1 = item.Report1,
                    Report2 = item.Report2
                };
                list.Add(user);
            }
            return list;
        }

        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            AddNewPeriod,
            UpdatePlanRecord,
            AddNewFactRecord,
            UpdateFactRecord,
            UsersAccessToReportsUpdated
        }

#endregion
    }
}