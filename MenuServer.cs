﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Core
{
    public record rootMenu(string ParentStaticRootID, string ContentID, string Header, int Priority);
    public static class RootMenus
    {
        public static List<rootMenu> Items = new List<rootMenu>();
        public static rootMenu? TryCreate(string ContentID)
        {
            return Items.FirstOrDefault((m) => m.ContentID == ContentID);
        }
    }

    public interface IMenuItemServer
    {
        public void Add(string ParentContentID, IEnumerable<MenuItemVM> Menus);
        public void Remove(string ContentID);
        public void Remove(IEnumerable<MenuItemVM> Menus);
        public bool Contains(string ContentID);
        public void UpdateSeparatorGroup(string ParentContentID);
        public void UpdateSeparatorGroup(MenuItemVM? ParentContentID);
    }

    public class MenuServer: PriorityServer, IMenuItemServer
    {
        public static ObservableCollection<MenuItemVM> Items = new ObservableCollection<MenuItemVM>();
        private MenuItemVM? Recur(IEnumerable<PriorityItem> root, Func<MenuItemVM, bool> TestFunc)
        {
            foreach (var m in root)
            {
                if (m is MenuItemVM mm)
                {
                    if (TestFunc(mm)) return mm;
                    else
                    {
                        var sm = Recur(mm.Items, TestFunc);
                        if (sm != null) return sm;
                    }
                }
            }
            return null;
        }
        private MenuItemVM? CreateStatandartMenu(rootMenu? rm)
        {
            if (rm == null) return null;
            (this as IMenuItemServer).Add(rm.ParentStaticRootID, new[]
            {
                new MenuItemVM
                {
                    Header = rm.Header,
                    ContentID =rm.ContentID,
                    Priority = rm.Priority,
                },
            });
            return Recur(Items, (i) => i.ContentID == rm.ContentID);
        }
        void IMenuItemServer.Add(string ParentContentID, IEnumerable<MenuItemVM> Menus)
        {
            var m = Recur(Items, (i) => i.ContentID == ParentContentID);
            m ??= CreateStatandartMenu(RootMenus.TryCreate(ParentContentID));
            if (m == null) return;
            base.Add(m.Items, Menus);
        }
        void IMenuItemServer.Remove(string ContentID)
        {
            MenuItemVM? item = null;
            var m = Recur(Items, (root) =>
            {
                foreach (var i in root.Items) if (i.ContentID == ContentID)
                    {
                        if (i is MenuItemVM ii)
                        {
                            item = ii;
                            return true;
                        }
                    }
                return false;
            });
            if (m != null && item != null) m.Items.Remove(item);
            UpdateSeparatorGroup(m);
        }
        public bool Contains(string ContentID)
        {
            return Recur(Items, (i) => i.ContentID == ContentID) != null;
        }

        public void UpdateSeparatorGroup(MenuItemVM? root)
        {
            if (root != null && root.Items.Count > 0) base.UpdateSeparatorGroup(root.Items);
        }
        public void UpdateSeparatorGroup(string ParentContentID)
        {
            var root = Recur(Items, (i) => i.ContentID == ParentContentID);
            UpdateSeparatorGroup(root);
        }

        public void Remove(IEnumerable<MenuItemVM> Menus)
        {
            foreach (var menu in Menus)
            {
                MenuItemVM? item = null;
                var m = Recur(Items, (root) =>
                {
                    foreach (var i in root.Items) if (i == menu)
                        {
                                item = menu;
                                return true;
                        }
                    return false;
                });
                if (m != null && item != null) m.Items.Remove(item);
                UpdateSeparatorGroup(m);
            }
        }
    }
}