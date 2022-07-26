using EFCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EFCore
{
    class Program
    {
        public static readonly DashTableContext context = new DashTableContext();
        public static List<Core.SubModule> SubModule = new List<Core.SubModule>();
        public static void Function()
        {
            using (DashTableContext cont = new DashTableContext())
            {

                List<Core.Module> ModuleList = new List<Core.Module>();
                var module = (from a in cont.Modules
                              join b in cont.ModuleDetails on a.ModuleId equals b.ModuleId
                              where b.Visibility == true
                              select new { a, b }).ToList();

                foreach (var item in module)
                {
                    ModuleList.Add(new Core.Module
                    {
                        ModuleId = item.a.ModuleId,
                        ModuleName = item.a.ModuleName,
                        Description = item.a.Description,
                        FriendlyName = item.a.FriendlyName,
                        InUse = (bool)item.b.Visibility,
                        GoogleAnalyticsName = item.b.GoogleAnalyticsName,
                        Url = item.b.Url,
                        AddedBy = item.a.AddedBy,
                        AddedOn = item.a.AddedOn,
                        PlatformId = item.a.PlatformId,
                        IconPath = item.b.IconPath,
                        SubModules = AddSubModules(cont.SubModules.Where(x => x.ModuleId == item.a.ModuleId).ToList())
                    });
                }
            }
        }
        public static List<Core.SubModule> AddSubModules(List<SubModule> subs)
        {
            if (subs != null)
            {
                //foreach (var item in subs)
                for (int item = 0; item < subs.Count; item++)
                {

                    SubModule.Add(new Core.SubModule()
                    {
                        ModuleId = subs[item].ModuleId,
                        SubModuleId = subs[item].SubModuleId,
                        SubModuleName = subs[item].SubModuleName,
                        Description = subs[item].Description,
                        FriendlyName = subs[item].FriendlyName,
                        Icon = subs[item].Icon,
                        DisplayOrder = subs[item].DisplayOrder,
                        AddedBy = (int)subs[item].AddedBy,
                        AddedOn = (DateTime)subs[item].AddedOn,
                        ParentSubModuleId = (int?)subs[item].ParentSubModuleId,
                        SubModules = new List<Core.SubModule>()
                        //SubModules = subs[item].ParentSubModuleId == null ? AddSubModules(context.SubModules.Where(x => x.ParentSubModuleId == subs[item].SubModuleId).ToList()) : null
                    });

                    NewMethod(subs, item);

                }
            }
            return SubModule;
        }

        private static void NewMethod(List<SubModule> subs, int item)
        {
            var a = context.SubModules.Where(x => x.ParentSubModuleId == subs[item].SubModuleId).ToList();
            if (a != null && a.Count > 0)
            {
                foreach (var i in a)
                {
                    Core.SubModule sub = new Core.SubModule();
                    sub.ModuleId = i.ModuleId;
                    sub.SubModuleId = i.SubModuleId;
                    sub.SubModuleName = i.SubModuleName;
                    sub.Description = i.Description;
                    sub.FriendlyName = i.FriendlyName;
                    sub.Icon = i.Icon;
                    sub.DisplayOrder = i.DisplayOrder;
                    sub.AddedBy = (int)i.AddedBy;
                    sub.AddedOn = (DateTime)i.AddedOn;
                    sub.ParentSubModuleId = (int?)i.ParentSubModuleId;
                    sub.SubModules = new List<Core.SubModule>();
                    if (!SubModule[item].SubModules.Contains(sub))
                        SubModule[item].SubModules.Add(sub);
                    NewMethod(subs, item);
                }
            }
        }

        static void Main(string[] args)
        {
            Function();
            Console.WriteLine("Hello World!");
            Console.ReadLine();

        }
    }
}
