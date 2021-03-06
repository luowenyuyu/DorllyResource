﻿using Newtonsoft.Json;
using Resource.Model;
using Resource.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
namespace Resource.Web.Controllers
{
    public class A_ParkController : BaseController
    {
        // GET: Park
        public ActionResult Index()
        {
            ViewBag.func = Func.GetFunc(user.Account, MenuPath);
            return View();
        }
        public JsonResult Search(SearchParam param)
        {

            var list = dc.Set<V_Park>().Where(a => ParkList.Contains(a.ID));
            if (!string.IsNullOrEmpty(param.Region)) list = list.Where(a => a.RegionID == param.Region);
            else if (!string.IsNullOrEmpty(param.City)) list = list.Where(a => a.CityID == param.City);
            if (!string.IsNullOrEmpty(param.ID)) list = list.Where(a => a.ID.Contains(param.ID));
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
        public JsonResult Create(T_Park park)
        {
            try
            {
                park.Enable = true;
                dc.Set<T_Park>().Add(park);
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

            var obj = dc.Set<V_Park>().Where(a => a.ID == id).FirstOrDefault();
            return View(obj);
        }
        [HttpPost]
        public JsonResult Edit(string id, FormCollection form)
        {
            try
            {

                T_Park park = dc.Set<T_Park>().Where(a => a.ID == id).FirstOrDefault();
                if (TryUpdateModel(park, "", form.AllKeys, new string[] { "Enable" }))
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

                T_Park park = dc.Set<T_Park>().Where(a => a.ID == id).FirstOrDefault();
                dc.Set<T_Park>().Remove(park);
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

                T_Park park = dc.Set<T_Park>().Where(a => a.ID == id).FirstOrDefault();
                park.Enable = true;
                dc.Set<T_Park>().AddOrUpdate(park);
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

                T_Park park = dc.Set<T_Park>().Where(a => a.ID == id).FirstOrDefault();
                park.Enable = false;
                dc.Set<T_Park>().AddOrUpdate(park);
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
