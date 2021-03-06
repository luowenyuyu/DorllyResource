﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Resource.Model;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Resource.Web.Models;
namespace Resource.Web.Controllers
{
    public class A_FloorController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.func = Func.GetFunc(user.Account, MenuPath);
            return View();
        }
        public JsonResult Search(SearchParam param)
        {

            var list = dc.Set<V_Floor>().Where(a => ParkList.Contains(a.ParkID));
            if (!string.IsNullOrEmpty(param.Build)) list = list.Where(a => a.BuildingID == param.Build);
            else if (!string.IsNullOrEmpty(param.Stage)) list = list.Where(a => a.StageID == param.Stage);
            else if (!string.IsNullOrEmpty(param.Park)) list = list.Where(a => a.ParkID == param.Park);
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
        public JsonResult Create(T_Floor floor)
        {
            try
            {
                floor.Enable = true;
                dc.Set<T_Floor>().Add(floor);
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

            var obj = dc.Set<V_Floor>().Where(a => a.ID == id).FirstOrDefault();
            return View(obj);
        }
        [HttpPost]
        public JsonResult Edit(string id, FormCollection form)
        {
            try
            {

                T_Floor floor = dc.Set<T_Floor>().Where(a => a.ID == id).FirstOrDefault();
                if (TryUpdateModel(floor, "", form.AllKeys, new string[] { "Enable" }))
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

                T_Floor floor = dc.Set<T_Floor>().Where(a => a.ID == id).FirstOrDefault();
                dc.Set<T_Floor>().Remove(floor);
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

                T_Floor floor = dc.Set<T_Floor>().Where(a => a.ID == id).FirstOrDefault();
                floor.Enable = true;
                dc.Set<T_Floor>().AddOrUpdate(floor);
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

                T_Floor floor = dc.Set<T_Floor>().Where(a => a.ID == id).FirstOrDefault();
                floor.Enable = false;
                dc.Set<T_Floor>().AddOrUpdate(floor);
                dc.SaveChanges();
                return Json(Result.Success());
            }
            catch (Exception ex)
            {
                return Json(Result.Exception(exmsg: ex.StackTrace));
            }
        }
        public JsonResult GetID(string pid)
        {
            string num = string.Empty;
            var count = dc.Set<T_Floor>().Where(a => a.BuildingID == pid).Count();
            count++;
            if (count < 10) num = "00" + count;
            else if (count < 100) num = "0" + count;
            else num = count.ToString();
            return Json(new { id = pid + "-" + num }, JsonRequestBehavior.AllowGet);
        }

    }
}
