﻿using Newtonsoft.Json;
using Resource.Model;
using Resource.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Resource.Web.Controllers
{
    public class PublicController : ResourceBusinessController
    {
        // GET: Public
        #region 首页
        public ActionResult Index(string rid)
        {
            ViewBag.id = rid;
            ViewBag.func = Func.GetFunc(user.Account, MenuPath);
            return View();
        }
        public ActionResult Check(string id)
        {
            var pub = dc.Set<T_ResourcePublic>().AsNoTracking().Where(a => a.ID == id).FirstOrDefault();
            ViewBag.check = 1;
            return Redirect(pub.ResourceID, pub);
        }
        public ContentResult Search(SearchParam param)
        {
            var list = dc.Set<V_Public>().Where(a => ParkList.Contains(a.Loc1));
            if (!string.IsNullOrEmpty(param.Park)) list = list.Where(a => a.Loc1 == param.Park);
            if (!string.IsNullOrEmpty(param.ID)) list = list.Where(a => a.ResourceID.Contains(param.ID));
            if (!string.IsNullOrEmpty(param.Name)) list = list.Where(a => a.ResourceName.Contains(param.Name));
            if (!string.IsNullOrEmpty(param.Group)) list = list.Where(a => a.GroupID == param.Group);
            if (param.Stime != null) list = list.Where(a=>a.UpdateTime>=param.Stime);
            if (param.Etime != null) list = list.Where(a => a.UpdateTime <= param.Etime);
            if (param.Kind != null) list = list.Where(a => a.ResourceKindID == param.Kind);
            if (param.Status != null) list = list.Where(a => a.Status == param.Status);
            int count = list.Count();
            list = list.OrderByDescending(a => a.UpdateTime).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize);
            JsonSerializerSettings setting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm"
            };
            var obj = JsonConvert.SerializeObject(new { count = count, data = list.ToList() }, setting);
            return Content(obj);
        }
        #endregion
        #region 发布业务
        public ActionResult PublicIndex()
        {
            return View();
        }
        public JsonResult PublicSearch(SearchParam param)
        {
            string[] pubList = dc.Set<T_ResourcePublic>().Where(a => a.Status != 6).Select(a => a.ResourceID).ToArray();
            var list = dc.Set<V_Resource>().Where(a => !pubList.Contains(a.ID));
            if (!string.IsNullOrEmpty(param.Park)) list = list.Where(a => a.Loc1 == param.Park);
            else list = list.Where(a => ParkList.Contains(a.Loc1));
            if (!string.IsNullOrEmpty(param.ID)) list = list.Where(a => a.ID.Contains(param.ID));
            if (!string.IsNullOrEmpty(param.Name)) list = list.Where(a => a.Name.Contains(param.Name));
            if (!string.IsNullOrEmpty(param.Group)) list = list.Where(a => a.GroupID == param.Group);
            if (param.Kind != null) list = list.Where(a => a.ResourceKindID == param.Kind);
            int count = list.Count();
            list = list.OrderBy(a => a.ResourceKindID).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize);
            return Json(new { count = count, data = list.ToList() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            return View("PublicIndex");
        }
        public ActionResult Edit(string id)
        {
            var pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 4 || pub.Status == 6 || pub.Status == 2)
                return Content("<script>window.parent.layer.closeAll();window.parent.layer.msg('该数据不允许编辑!');</script>");
            return Redirect(pub.ResourceID, pub);
        }
        public ActionResult Public(string id)
        {
            T_ResourcePublic pub = new T_ResourcePublic()
            {
                ID = DateTime.Now.ToString("yyyyMMddHHmmsss"),
                BeginTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1)
            };
            return Redirect(id, pub);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddOrUpdate(string id, FormCollection form)
        {
            string pubID = form["PublicID"];
            return SavePubInfo(pubID, id, form);
        }
        public ActionResult Redirect(string resourceID, T_ResourcePublic pub)
        {
            var kind = dc.Set<T_Resource>().Where(a => a.ID == resourceID).FirstOrDefault().ResourceKindID;
            ViewBag.price = dc.Set<T_ResourcePrice>().Where(a => a.ResourceID == resourceID).FirstOrDefault() ?? new T_ResourcePrice();
            ViewBag.img = dc.Set<T_ResourceImg>().Where(a => a.ResourceID == resourceID).ToList();
            ViewBag.resource = dc.Set<V_Resource>().Where(a => a.ID == resourceID).FirstOrDefault();
            switch (kind)
            {
                case 1:
                    //ViewBag.resource = dc.Set<V_RS_Info>().Where(a => a.ID == resourceID).FirstOrDefault();
                    return View("PublicRM", pub);
                case 2:
                    //ViewBag.resource = dc.Set<V_RS_Info>().Where(a => a.ID == resourceID).FirstOrDefault();
                    return View("PublicCB", pub);
                case 3:
                    //ViewBag.resource = dc.Set<V_RS_Info>().Where(a => a.ID == resourceID).FirstOrDefault();
                    return View("PublicMR", pub);
                case 4:
                    //ViewBag.resource = dc.Set<V_RS_Info>().Where(a => a.ID == resourceID).FirstOrDefault();
                    return View("PublicAD", pub);
            }
            return View("error");
        }
        #endregion

        #region 状态业务
        public JsonResult Pass(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 2) return Json(Result.Fail(msg: "状态一致，无从改变！"));
            else if (pub.Status != 1) return Json(Result.Fail(msg: "请选择正确的操作！"));
            pub.Status = 2;
            pub.UpdateTime = DateTime.Now;
            pub.UpdateUser = user.Account;
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        public JsonResult NPass(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 3) return Json(Result.Fail(msg: "状态一致，无从改变！"));
            else if (pub.Status != 1) return Json(Result.Fail(msg: "请选择正确的操作！"));
            pub.Status = 3;
            pub.UpdateTime = DateTime.Now;
            pub.UpdateUser = user.Account;
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        public JsonResult Pub(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 4) return Json(Result.Fail(msg: "状态一致，无从改变！"));
            else if (pub.Status != 2 && pub.Status != 5) return Json(Result.Fail(msg: "请选择正确的操作！"));
            pub.Status = 4;
            pub.UpdateTime = DateTime.Now;
            pub.UpdateUser = user.Account;
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        public JsonResult NPub(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 5) return Json(Result.Fail(msg: "状态一致，无从改变！"));
            else if (pub.Status != 4) return Json(Result.Fail(msg: "请选择正确的操作！"));
            pub.Status = 5;
            pub.UpdateTime = DateTime.Now;
            pub.UpdateUser = user.Account;
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        public JsonResult Off(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 6) return Json(Result.Fail(msg: "状态一致，无从改变！"));
            else if (pub.Status == 4) return Json(Result.Fail(msg: "请选择正确的操作！"));
            pub.Status = 6;
            pub.UpdateTime = DateTime.Now;
            pub.UpdateUser = user.Account;
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        public JsonResult Del(string id)
        {
            T_ResourcePublic pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
            if (pub.Status == 4) return Json(Result.Fail(msg: "请选择正确的操作！"));
            dc.Set<T_ResourcePublic>().Remove(pub);
            if (dc.SaveChanges() > 0) return Json(Result.Success());
            return Json(Result.Fail());
        }
        #endregion
        #region createodld
        //public ActionResult CreatePub(string id)
        //{
        //    T_ResourcePublic pub = new T_ResourcePublic() { ID = DateTime.Now.ToString("yyyyMMddHHmmsss") };
        //    return Redirect(id, pub);
        //    //var kind = dc.Set<T_Resource>().Where(a => a.ID == id).FirstOrDefault();
        //    //ViewBag.price = dc.Set<T_ResourcePrice>().Where(a => a.ResourceID == id).FirstOrDefault();
        //    //ViewBag.img = dc.Set<T_ResourceImg>().Where(a => a.ResourceID == id).ToList();
        //    //switch (kind)
        //    //{
        //    //    case 1:
        //    //        ViewBag.resource = dc.Set<V_RM>().Where(a => a.ID == id).FirstOrDefault();
        //    //        return View("CreateRM", pub);
        //    //    case 2:
        //    //        ViewBag.resource = dc.Set<V_CB>().Where(a => a.ID == id).FirstOrDefault();
        //    //        return View("CreateCB", pub);
        //    //    case 3:
        //    //        ViewBag.resource = dc.Set<V_MR>().Where(a => a.ID == id).FirstOrDefault();
        //    //        return View("CreateMR", pub);
        //    //    case 4:
        //    //        ViewBag.resource = dc.Set<V_AD>().Where(a => a.ID == id).FirstOrDefault();
        //    //        return View("CreateAD", pub);
        //    //}
        //    //return View();
        //}
        //public ActionResult Edit(string id)
        //{
        //    var pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
        //    return Redirect(pub.ResourceID, pub);
        //    //ViewBag.price = dc.Set<T_ResourcePrice>().Where(a => a.ResourceID == resourceID).FirstOrDefault();
        //    //ViewBag.img = dc.Set<T_ResourceImg>().Where(a => a.ResourceID == resourceID).ToList();
        //    //switch (pub.T_Resource.ResourceKindID)
        //    //{
        //    //    case 1:
        //    //        ViewBag.resource = dc.Set<V_RM>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        return View("EditRM", pub);
        //    //    case 2:
        //    //        ViewBag.resource = dc.Set<V_CB>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        return View("EditCB", pub);
        //    //    case 3:
        //    //        ViewBag.resource = dc.Set<V_MR>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        return View("EditMR", pub);
        //    //    case 4:
        //    //        ViewBag.resource = dc.Set<V_AD>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        return View("EditAD", pub);
        //    //}
        //    //return View();
        //} 
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Create(string id, FormCollection form)
        //{
        //    string pubID = form["PublicID"];
        //    return SavePubInfo(pubID, id, form);
        //    //try
        //    //{
        //    //    T_ResourcePublic pub = new T_ResourcePublic();
        //    //    var resourceID = form["ID"];
        //    //    if (string.IsNullOrEmpty(resourceID)) return Json(ResponseResult.GetResult(ResultEnum.Fail));
        //    //    resourceID = resourceID;
        //    //    pub.Status = 1;
        //    //    pub.CreateTime = DateTime.Now;
        //    //    pub.CreateUser = user.Account;
        //    //    pub.UpdateTime = DateTime.Now;
        //    //    pub.UpdateUser = user.Account;
        //    //    if (TryUpdateModel(pub, "", form.AllKeys))
        //    //    {
        //    //        if (string.IsNullOrEmpty(form["PublicID"])) pub.ID = Guid.NewGuid().ToString();
        //    //        else pub.ID = form["PublicID"];
        //    //        dc.Set<T_ResourcePublic>().Add(pub);
        //    //        var rs = dc.Set<T_Resource>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        if (TryUpdateModel(rs, "", form.AllKeys, new string[] { "ID" }))
        //    //        {
        //    //            if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
        //    //        }
        //    //    }
        //    //    return Json(ResponseResult.GetResult(ResultEnum.Fail));
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    System.Diagnostics.Debug.Print(ex.ToString());
        //    //    return Json(ResponseResult.GetResult(ResultEnum.Exception));
        //    //}
        //}
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Edit(string id, FormCollection form)
        //{
        //    string pubID = form["PublicID"];
        //    return SavePubInfo(pubID, id, form);
        //    //string id = form["PublicID"];
        //    //try
        //    //{
        //    //    var pub = dc.Set<T_ResourcePublic>().Where(a => a.ID == id).FirstOrDefault();
        //    //    pub.UpdateTime = DateTime.Now;
        //    //    pub.UpdateUser = user.Account;
        //    //    if (TryUpdateModel(pub, "", form.AllKeys, new string[] { "ID" }))
        //    //    {
        //    //        var rs = dc.Set<T_Resource>().Where(a => a.ID == resourceID).FirstOrDefault();
        //    //        if (TryUpdateModel(rs, "", form.AllKeys, new string[] { "ID" }))
        //    //        {
        //    //            if (dc.SaveChanges() > 0) return Json(ResponseResult.GetResult(ResultEnum.Success));
        //    //        }
        //    //    }
        //    //    return Json(ResponseResult.GetResult(ResultEnum.Fail));
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    System.Diagnostics.Debug.Print(ex.ToString());
        //    //    return Json(ResponseResult.GetResult(ResultEnum.Exception));
        //    //}
        //}
        #endregion
    }
}