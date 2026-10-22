using ExpenseTrackerSystem.Data;
using ExpenseTrackerSystem.Models;
using ExpenseTrackerSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExpenseTrackerSystem.Controllers
{
    public class SetupController : Controller
    {
        private readonly ExpenseDbContext db = new ExpenseDbContext();

        // =========================================================
        // STEP 1
        // =========================================================

        public ActionResult Index()
        {
            // If setup is already completed,
            // don't show setup wizard again.
            if (IsSetupCompleted())
            {
                return RedirectToExpenseDashboard();
            }

            var model = new SetupViewModel();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.RoomName))
            {
                ModelState.AddModelError(
                    "RoomName",
                    "Please enter room/group name.");

                return View(model);
            }

            TempData["RoomName"] = model.RoomName.Trim();

            return RedirectToAction("Users");
        }


        // =========================================================
        // STEP 2 - ADD USERS
        // =========================================================

        public ActionResult Users()
        {
            var roomName = TempData["RoomName"] as string;

            if (string.IsNullOrWhiteSpace(roomName))
            {
                return RedirectToAction("Index");
            }

            TempData.Keep("RoomName");

            var model = new SetupViewModel
            {
                RoomName = roomName,

                Users = new List<SetupUserViewModel>
                {
                    new SetupUserViewModel(),
                    new SetupUserViewModel(),
                    new SetupUserViewModel(),
                    new SetupUserViewModel()
                }
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Users(SetupViewModel model)
        {
            var roomName = model.RoomName;

            if (string.IsNullOrWhiteSpace(roomName))
            {
                roomName = TempData["RoomName"] as string;
            }

            if (string.IsNullOrWhiteSpace(roomName))
            {
                return RedirectToAction("Index");
            }

            if (model.Users == null)
            {
                ModelState.AddModelError("", "Please enter members.");

                return View(model);
            }

            // Remove empty rows
            var validUsers = model.Users
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .ToList();

            if (validUsers.Count < 2)
            {
                ModelState.AddModelError("", "At least 2 members are required.");

                model.RoomName = roomName;

                return View(model);
            }

            // Check duplicate names
            var duplicateNames = validUsers
                .GroupBy(x => x.Name.Trim().ToLower())
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                ModelState.AddModelError("", "Member names must be unique.");

                model.RoomName = roomName;

                return View(model);
            }

            try
            {
                // -------------------------------------------------
                // Save users first
                // -------------------------------------------------

                var createdUsers = new List<SetupUserViewModel>();

                foreach (var item in validUsers)
                {
                    var name = item.Name.Trim();

                    var existingUser = db.Users
                        .FirstOrDefault(x =>
                            x.Name.ToLower() == name.ToLower());

                    User user;

                    if (existingUser != null)
                    {
                        user = existingUser;
                    }
                    else
                    {
                        user = new User
                        {
                            Name = name,
                            Username = item.Username,
                            MobileNo = item.MobileNo,
                            UPIId = item.UPIId,
                            Password = null,
                            IsActive = 1
                        };

                        db.Users.Add(user);

                        db.SaveChanges();
                    }

                    createdUsers.Add(
                        new SetupUserViewModel
                        {
                            UserId = user.UserId,
                            Name = user.Name,
                            Username = user.Username,
                            MobileNo = user.MobileNo,
                            UPIId = user.UPIId
                        });
                }

                // -------------------------------------------------
                // Store information for next step
                // -------------------------------------------------

                TempData["RoomName"] = roomName;
                TempData["SetupUsers"] = createdUsers;

                return RedirectToAction("Groups");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save members: " + ex.Message);

                model.RoomName = roomName;

                return View(model);
            }
        }


        // =========================================================
        // STEP 3 - GENERATE GROUPS
        // =========================================================

        public ActionResult Groups()
        {
            var users =
                TempData["SetupUsers"]
                as List<SetupUserViewModel>;

            if (users == null || users.Count < 2)
            {
                return RedirectToAction("Users");
            }

            var roomName =
                TempData["RoomName"] as string;

            TempData.Keep("SetupUsers");
            TempData.Keep("RoomName");

            var groups =
                GenerateCombinations(users);

            var model = new SetupViewModel
            {
                RoomName = roomName,
                Users = users,
                Groups = groups
            };

            return View(model);
        }


        // =========================================================
        // SAVE SELECTED GROUPS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Groups(SetupViewModel model)
        {
            var users = TempData["SetupUsers"] as List<SetupUserViewModel>;

            if (users == null || users.Count < 2)
            {
                return RedirectToAction("Users");
            }

            var roomName = TempData["RoomName"] as string;

            if (model.Groups == null || !model.Groups.Any(x => x.IsSelected))
            {
                ModelState.AddModelError("", "Please select at least one group.");

                model.RoomName = roomName;
                model.Users = users;
                model.Groups = GenerateCombinations(users);

                return View(model);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // ---------------------------------------------
                    // Save selected groups
                    // ---------------------------------------------

                    foreach (var selectedGroup in model.Groups.Where(x => x.IsSelected))
                    {
                        var userIds = selectedGroup.UserIds.Distinct().ToList();

                        if (userIds.Count < 2)
                            continue;

                        // Check whether same group already exists
                        var existingGroup = db.UserGroups.FirstOrDefault(x => x.GroupName == selectedGroup.GroupName);

                        UserGroup group;

                        if (existingGroup != null)
                        {
                            group = existingGroup;
                        }
                        else
                        {
                            group = new UserGroup
                            {
                                GroupName = selectedGroup.GroupName,

                                // Keep for compatibility
                                Members = selectedGroup.GroupName
                            };

                            db.UserGroups.Add(group);

                            db.SaveChanges();
                        }

                        // -----------------------------------------
                        // Save Group Members
                        // -----------------------------------------
                        
                        foreach (var userId in userIds)
                        {
                            bool alreadyMember = db.UserGroupMembers.Any(x => x.GroupId == group.GroupId && x.UserId == userId);

                            if (!alreadyMember)
                            {
                                db.UserGroupMembers.Add(
                                    new UserGroupMember
                                    {
                                        GroupId = group.GroupId,

                                        UserId = userId
                                    });
                            }
                        }

                        db.SaveChanges();
                    }

                    transaction.Commit();

                    SetSetupCompleted();

                    TempData.Clear();

                    return RedirectToAction("Complete");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    ModelState.AddModelError("", "Unable to save groups: " + ex.Message);

                    model.RoomName = roomName;
                    model.Users = users;

                    model.Groups = GenerateCombinations(users);

                    return View(model);
                }
            }
        }


        // =========================================================
        // STEP 4 - COMPLETE
        // =========================================================

        public ActionResult Complete()
        {
            if (!IsSetupCompleted())
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        // =========================================================
        // COMBINATION GENERATOR
        // =========================================================

        private List<GroupOptionViewModel>
            GenerateCombinations(
                List<SetupUserViewModel> users)
        {
            var result =
                new List<GroupOptionViewModel>();

            for (int size = 2;
                 size <= users.Count;
                 size++)
            {
                GenerateCombinationRecursive(
                    users,
                    size,
                    0,
                    new List<SetupUserViewModel>(),
                    result);
            }

            return result;
        }


        private void GenerateCombinationRecursive(
            List<SetupUserViewModel> users,
            int targetSize,
            int startIndex,
            List<SetupUserViewModel> current,
            List<GroupOptionViewModel> result)
        {
            if (current.Count == targetSize)
            {
                result.Add(
                    new GroupOptionViewModel
                    {
                        GroupName =
                            string.Join(
                                " + ",
                                current.Select(
                                    x => x.Name.Trim())),

                        UserIds =
                            current.Select(
                                x => x.UserId).ToList(),

                        IsSelected = false
                    });

                return;
            }

            for (int i = startIndex;
                 i < users.Count;
                 i++)
            {
                current.Add(users[i]);

                GenerateCombinationRecursive(
                    users,
                    targetSize,
                    i + 1,
                    current,
                    result);

                current.RemoveAt(
                    current.Count - 1);
            }
        }


        // =========================================================
        // SETUP STATUS
        // =========================================================

        private bool IsSetupCompleted()
        {
            var value = System.Configuration.ConfigurationManager
                .AppSettings["ExpenseTrackerSetupCompleted"];

            return string.Equals(
                value,
                "true",
                StringComparison.OrdinalIgnoreCase);
        }


        // =========================================================
        // DASHBOARD REDIRECT
        // =========================================================

        private ActionResult RedirectToExpenseDashboard()
        {
            // IMPORTANT:
            // Change "Expense" to your actual
            // Expense Dashboard controller name.

            return RedirectToAction("Index", "Home");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private void SetSetupCompleted()
        {
            var config =
                System.Web.Configuration.WebConfigurationManager
                    .OpenWebConfiguration("~");

            var setting =
                config.AppSettings.Settings[
                    "ExpenseTrackerSetupCompleted"];

            if (setting == null)
            {
                config.AppSettings.Settings.Add(
                    "ExpenseTrackerSetupCompleted",
                    "true");
            }
            else
            {
                setting.Value = "true";
            }

            config.Save(
                System.Configuration.ConfigurationSaveMode.Modified);

            System.Configuration.ConfigurationManager
                .RefreshSection("appSettings");
        }
    }
}