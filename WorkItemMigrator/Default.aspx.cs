using System;
using WebFormsMvp.Web;
using WorkItemMigrator.Models;
using WorkItemMigrator.Views;

namespace WorkItemMigrator
{ 
    public partial class DefaultView : MvpPage<DefaultModel>, IDefaultView
    {
        public event EventHandler Cancel;
        public void OnCancel(EventArgs e)
        {
            var handler = Cancel;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        protected void CancelHandler(object sender, EventArgs e)
        {
            OnCancel(e);
        }
    }
}