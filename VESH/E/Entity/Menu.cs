using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.E.Entity {
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu:GCL.Db.Ni.IRowToObject {
        public string Name { get; set; }
        public int MenuLevel { get; set; }
        public string PageIndex { get; set; }
        public string PageID { get; set; }
        public string URLFormatting { get; set; }
        public string Target { get; set; }

        public void Fill(System.Data.DataRow row) {
            this.Name = Convert.ToString(row["Name"]);
            this.MenuLevel = Convert.ToInt32(row["MenuLevel"]);
            this.PageIndex = Convert.ToString(row["PageIndex"]);
            this.PageID = Convert.ToString(row["PageID"]);
            this.URLFormatting = Convert.ToString(row["URLFormatting"]);
            this.Target = Convert.ToString(row["Target"]);
        }
    }
}