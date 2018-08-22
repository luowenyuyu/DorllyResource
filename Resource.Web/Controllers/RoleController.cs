﻿
using Resource.Model;
using Resource.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Resource.Web.Controllers
{
    public class RoleController : BaseController
    {

        // GET: Role
        public ActionResult Index()
        {
            string menuName = "/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"];
            List<T_RoleFunc> rmfList = new FuncView().GetFunc(user, menuName);
            return View(rmfList);
        }


        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(T_Role role)
        {
            try
            {

                role.Enable = true;
                dc.Set<T_Role>().Add(role);
                if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }

        }
        public ActionResult Edit(int id)
        {

            T_Role role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
            return View(role);
        }
        [HttpPost]
        public JsonResult Edit(int id, FormCollection form)
        {

            try
            {

                T_Role role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
                if (TryUpdateModel(role, "", form.AllKeys, new string[] { "Enable" }))
                {
                    if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                    else return Json(ResponseResult.GetResult(ResultEnum.Fail));
                }
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }
        }
        [HttpPost]
        public JsonResult Del(int id)
        {
            try
            {

                T_Role role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
                dc.Set<T_Role>().Remove(role);
                if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }
        }
        public JsonResult Open(int id)
        {
            try
            {

                T_Role role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
                role.Enable = true;
                dc.Set<T_Role>().AddOrUpdate(role);
                if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }
        }

        public JsonResult Close(int id)
        {
            try
            {

                T_Role role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
                role.Enable = false;
                dc.Set<T_Role>().AddOrUpdate(role);
                if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }
        }
        public ActionResult Func(int id)
        {

            var role = dc.Set<T_Role>().Where(a => a.ID == id).FirstOrDefault();
            ViewBag.role = role;
            ViewBag.order = 1;
            var list = dc.Set<T_Menu>().ToList();
            return View(list);
        }
        [HttpPost]
        public JsonResult Func(int id, List<T_RoleMenu> rmList, List<T_RoleFunc> rmfList)
        {
            try
            {

                if (id == 0) return Json(ResponseResult.GetResult(ResultEnum.Fail));
                //
                var menuList = dc.Set<T_RoleMenu>().Where(a => a.RoleID == id);
                foreach (var item in menuList)
                {
                    dc.Set<T_RoleMenu>().Remove(item);
                }
                if (rmList != null)
                {
                    foreach (var item in rmList)
                    {
                        dc.Set<T_RoleMenu>().Add(item);
                    }
                }

                //
                var funcList = dc.Set<T_RoleFunc>().Where(a => a.RoleID == id);
                foreach (var item in funcList)
                {
                    dc.Set<T_RoleFunc>().Remove(item);
                }
                if (rmfList != null)
                {
                    foreach (var item in rmfList)
                    {
                        dc.Set<T_RoleFunc>().Add(item);
                    }
                }


                //if (rmList == null && rmfList == null) return Json(ResponseResult.GetResult(ResultEnum.Nullable));


                //if (rmList != null)
                //{
                //    var menuList = dc.Set<T_RoleMenu>().Where(a => a.RoleID == id);
                //    foreach (var item in menuList)
                //    {
                //        dc.Set<T_RoleMenu>().Remove(item);
                //    }
                //    foreach (var item in rmList)
                //    {
                //        dc.Set<T_RoleMenu>().Add(item);
                //    }
                //}
                //if (rmfList != null)
                //{
                //    var funcList = dc.Set<T_RoleFunc>().Where(a => a.RoleID == id);
                //    foreach (var item in funcList)
                //    {
                //        dc.Set<T_RoleFunc>().Remove(item);
                //    }
                //    foreach (var item in rmfList)
                //    {
                //        dc.Set<T_RoleFunc>().Add(item);
                //    }
                //}

                if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
                else return Json(ResponseResult.GetResult(ResultEnum.Fail));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return Json(ResponseResult.GetResult(ResultEnum.Exception));
            }
        }
        public JsonResult Search(SearchParam param)
        {

            var list = dc.Set<T_Role>().Where(a => true).Select(a => new { a.ID, a.Name, a.Description, a.Enable });
            if (!string.IsNullOrEmpty(param.Name)) list = list.Where(a => a.Name.Contains(param.Name));
            int count = list.Count();
            list = list.OrderBy(a => a.ID).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize);
            return Json(new { count = count, data = list.ToList() }, JsonRequestBehavior.AllowGet);
        }

    }
}