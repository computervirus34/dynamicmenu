using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicMenyBind.DataModel;
using DynamicMenyBind.Models;
using System.Web.Security;
namespace DynamicMenyBind.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModels _login)
        {
            if (ModelState.IsValid) //validating the user inputs
            {
                bool isExist = false;
                using (SampleMenuMasterDBEntities _entity = new SampleMenuMasterDBEntities())  // out Entity name is "SampleMenuMasterDBEntites"
                { 
                    isExist = _entity.tblLogins.Where(x => x.UserName.Trim().ToLower() == _login.UserName.Trim().ToLower() && x.Password.Equals(_login.Password)).Any(); //validating the user name in tblLogin table whether the user name is exist or not
                    if (isExist) 
                    {
                        LoginModels _loginCredentials= _entity.tblLogins.Where
                            (x => x.UserName.Trim().ToLower() == _login.UserName.Trim().ToLower()).Select
                            (x => new LoginModels
                        {
                            UserName=x.UserName,
                            RoleName=x.tblRole.Roles,
                            UserRoleId=x.RoleId,
                            UserId=x.Id
                        }).FirstOrDefault();  // Get the login user details and bind it to LoginModels class
                        List<MenuModels> _menus = _entity.tblSubMenus.Where(x => x.RoleId == _loginCredentials.UserRoleId).Select(x => new MenuModels
                        {
                            MainMenuId=x.tblMainMenu.Id,
                            MainMenuName=x.tblMainMenu.MainMenu,
                            SubMenuId=x.Id,
                            SubMenuName=x.SubMenu,
                            ControllerName=x.Controller,
                            ActionName=x.Action,
                            RoleId=x.RoleId,
                            RoleName=x.tblRole.Roles
                        }).ToList(); //Get the Menu details from entity and bind it in MenuModels list.
                        FormsAuthentication.SetAuthCookie(_loginCredentials.UserName, false); // set the formauthentication cookie
                        Session["LoginCredentials"] = _loginCredentials; // Bind the _logincredentials details to "LoginCredentials" session
                        Session["MenuMaster"] = _menus; //Bind the _menus list to MenuMaster session
                        Session["UserName"] = _loginCredentials.UserName;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Please enter the valid credentials!...";
                        return View();
                    }
                }
            }
            return View();
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login", "Login");
        }
    }
}
