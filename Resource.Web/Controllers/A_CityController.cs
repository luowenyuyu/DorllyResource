﻿using Resource.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Resource.Web.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
namespace Resource.Web.Controllers
{
    public class A_CityController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.func = Func.GetFunc(user.Account, MenuPath);
            return View();
        }
        public JsonResult Search(SearchParam param)
        {
            var list = dc.Set<T_City>().Where(a => true);
            if (!string.IsNullOrEmpty(param.Name)) list = list.Where(a => a.Name.Contains(param.Name));
            if (param.Enable != null) list = list.Where(a => a.Enable == param.Enable);
            int count = list.Count();
            list = list.OrderBy(a => a.ID).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize);
            return Json(new { count = count, data = list.ToList() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(T_City city)
        {
            try
            {
                city.Enable = true;
                city.IsDefault = false;
                dc.Set<T_City>().Add(city);
                dc.SaveChanges();
                return Json(Result.Success());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }
        public ActionResult Edit(string id)
        {

            var entity = dc.Set<T_City>().Where(a => a.ID == id).FirstOrDefault();
            return View(entity);
        }
        [HttpPost]
        public JsonResult Edit(string id, FormCollection form)
        {
            try
            {

                T_City city = dc.Set<T_City>().Where(a => a.ID == id).FirstOrDefault();
                if (TryUpdateModel(city, "", form.AllKeys, new string[] { "Enable", "IsDefault" }))
                {
                    dc.SaveChanges();
                    return Json(Result.Success());
                }
                return Json(Result.Fail());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }
        [HttpPost]
        public JsonResult Del(string id)
        {
            try
            {

                T_City city = dc.Set<T_City>().Where(a => a.ID == id).FirstOrDefault();
                dc.Set<T_City>().Remove(city);
                dc.SaveChanges();
                return Json(Result.Success());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }
        [HttpPost]
        public JsonResult Open(string id)
        {
            try
            {

                T_City city = dc.Set<T_City>().Where(a => a.ID == id).FirstOrDefault();
                city.Enable = true;
                dc.Set<T_City>().AddOrUpdate(city);
                dc.SaveChanges();
                return Json(Result.Success());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }
        [HttpPost]
        public JsonResult Close(string id)
        {
            try
            {

                T_City city = dc.Set<T_City>().Where(a => a.ID == id).FirstOrDefault();
                city.Enable = false;
                dc.Set<T_City>().AddOrUpdate(city);
                dc.SaveChanges();
                return Json(Result.Success());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }

    }
}
